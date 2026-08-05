Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModello.Utility_Agenda
Imports AgronicaCoreModelsSTD.costanti
Imports System.Transactions
Imports AgronicaCoreModelsSTD.attivita.Attivita
Imports AgronicaCoreModello
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreContabDAL
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo
Imports AgronicaCoreMapper.My.Resources.AgronicaCoreMapper
Imports AgronicaCoreModelsSTD.exceptions

Public Class ModificaMultipla_Attivita
    Public Function ModificaMultipla_ListaAttivita(Tipo_Modifica As enum_ModificaMultiplaOperazioni,
                                                   lista_Attivita As List(Of Attivita_xModificaMultipla),
                                                   lista_Risorse As List(Of Risorsa),
                                                   Magazzino As Fabbricato,
                                                   Elimina_Precedenti As Boolean?,
                                                   objParametri_Server As AgronicaCoreParametri,
                                                   objParametri_Utenti As AgronicaCoreParametri
                                                   ) As List(Of ErroreGias)


        Dim lista_Errori As New List(Of ErroreGias)
        Dim currentAttivitaDes As String = ""
        Dim msg As String = ""

        Try

            If check_SelezionatoAlmenoUnElemento(lista_Attivita.Count, 0, msg) <> "" Then
                lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", msg, ""))
                Return lista_Errori
            End If

            ModificaMultipla_Attivita(Tipo_Modifica, lista_Attivita,
                                      lista_Risorse, Magazzino,
                                      Elimina_Precedenti, lista_Errori,
                                      objParametri_Server,
                                      objParametri_Utenti)

            If Not IsNothing(lista_Errori) AndAlso lista_Errori.FindAll(Function(c) (c.severity = ErroreGias_Severity.Bloccante)).Count > 0 Then
                Return lista_Errori
            End If

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            Throw New Exception(currentAttivitaDes & ": " & ex.Message, ex)
        End Try

        Return lista_Errori

    End Function

    Public Sub ModificaMultipla_Attivita(Tipo_Modifica As enum_ModificaMultiplaOperazioni,
                                         lista_Attivita As List(Of Attivita_xModificaMultipla),
                                         Lista_Risorse As List(Of Risorsa),
                                         Magazzino As Fabbricato,
                                         Elimina_Precedenti As Boolean?,
                                         ByRef lista_Errori As List(Of ErroreGias),
                                         objParametri_Server As AgronicaCoreParametri,
                                         objParametri_Utenti As AgronicaCoreParametri)

        Dim nOpModificate As Integer = 0
        Dim errMess As New List(Of String)
        Dim magazziniGiaAssociati As New List(Of String)
        Dim operazioniNoMagazzino As New List(Of String)

        For Each attivita In lista_Attivita

            Dim Id_Agenda As Integer = attivita.ID_Agenda
            Dim Raccoglitore_Cod As Integer = attivita.Raccoglitore_Cod
            Dim Lav_Cod As Integer = attivita.Lav_Cod
            Dim Piva As String = attivita.Piva
            Dim Sa_Cod As Integer = attivita.Sa_Cod
            Dim Data As Date = CDate(attivita.Data)

            Dim currentAttivitaDes As String = attivita.Lav_Des
            Dim msg As String = ""

            Dim InfoOperazione As InfoOperazione = GetInfoOperazione(attivita.Lav_Cod, Tipo_Attivita.QuadernoDiCampagna)

            Select Case Tipo_Modifica
                Case enum_ModificaMultiplaOperazioni.AggiungiMacchina
                    Dim lista_Macchine As List(Of RisorsaMacchina) =
                                Lista_Risorse.FindAll(Function(c) (c.classType = ClassType.RisorsaMacchina)).ConvertAll(Function(obj1) CType(obj1, risorse.RisorsaMacchina))

                    If check_SelezionatoAlmenoUnElemento(lista_Macchine.Count, enum_ModificaMultiplaOperazioni.AggiungiMacchina, msg) <> "" Then
                        lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", msg, ""))
                        Exit Sub
                    End If

                    If AggiungiMacchine(InfoOperazione, currentAttivitaDes, lista_Macchine, Id_Agenda, Piva, Sa_Cod, Data, Elimina_Precedenti, errMess, objParametri_Server) Then
                        nOpModificate += 1
                    End If

                Case enum_ModificaMultiplaOperazioni.EliminaMacchine

                    If EliminaMacchine(Id_Agenda, Piva, currentAttivitaDes, errMess, objParametri_Server) Then
                        nOpModificate += 1
                    End If

                Case enum_ModificaMultiplaOperazioni.AggiungiContatto
                    Dim lista_Persone As List(Of risorse.RisorsaPersona) =
                                Lista_Risorse.FindAll(Function(c) (c.classType = ClassType.RisorsaPersona)).ConvertAll(Function(obj1) CType(obj1, risorse.RisorsaPersona))

                    If check_SelezionatoAlmenoUnElemento(lista_Persone.Count, enum_ModificaMultiplaOperazioni.AggiungiContatto, msg) <> "" Then
                        lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", msg, ""))
                        Exit Sub
                    End If

                    If AggiungiContatti(InfoOperazione, currentAttivitaDes, lista_Persone, Id_Agenda, Piva, Sa_Cod, Data, Elimina_Precedenti, errMess, objParametri_Server) Then
                        nOpModificate += 1
                    End If

                Case enum_ModificaMultiplaOperazioni.EliminaContatti

                    If EliminaContatti(Id_Agenda, Piva, Sa_Cod, currentAttivitaDes, errMess, objParametri_Server) Then
                        nOpModificate += 1
                    End If

                Case enum_ModificaMultiplaOperazioni.AggiungiMagazzino

                    If IsNothing(Magazzino.primaryKey.codice) OrElse Magazzino.primaryKey.codice = 0 Then
                        lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", Gias.SelezionareUnMagazzino & "!", ""))
                        Exit Sub
                    End If

                    Dim Fabbricato_Cod As Integer = Magazzino.primaryKey.codice
                    Dim Piva_Fabbricato As String = Magazzino.primaryKey.centroAziendalePK.partitaIva
                    Dim Sa_Cod_Fabbricato As Integer = Magazzino.primaryKey.centroAziendalePK.codice

                    'leggo IMPOSTAZIONE UTENTE BLOCCA_SE_SUPERA_GIACENZE
                    Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                    Dim Impostazione_Valore_1 As String = CInt(objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE, objParametri_Utenti.UtenteUsername, objParametri_Utenti))

                    Dim BLOCCA_SE_SUPERA_GIACENZE As Boolean = False
                    Select Case Impostazione_Valore_1
                        Case "1"
                            BLOCCA_SE_SUPERA_GIACENZE = True
                    End Select

                    Select Case Lav_Cod
                        Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                             LAVCOD_DISERBO, LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                             LAVCOD_DISTRIBUZIONE_INSETTI, LAVCOD_CONFUSIONE_SESSUALE,
                             LAVCOD_DISORIENTAMENTO_SESSUALE, LAVCOD_CATTURE_MASSA,
                             LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISSECCAMENTO,
                             LAVCOD_TRATTAMENTO_POST_RACCOLTA, LAVCOD_INSTALLAZIONE_TRAPPOLE,
                             LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_FERTIRRIGAZIONE,
                             LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_CONCIMAZIONE_FOGLIARE,
                             LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_SARCHIATURA_CONCIMAZIONE,
                             LAVCOD_TRATTAMENTO_ANTIBUTTERATURA, LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA, LAVCOD_REINNESCO_TRAPPOLE

                            If AggiungiMagazzino(BLOCCA_SE_SUPERA_GIACENZE, Id_Agenda, Piva, Fabbricato_Cod, Piva_Fabbricato, Sa_Cod_Fabbricato, errMess, magazziniGiaAssociati, objParametri_Server) Then
                                nOpModificate += 1
                            End If
                        Case Else
                            'Dim mess = "- " & String.Format(NonPossibileAggiungereMagazzinoOperazioniDiTipoX, currentAttivitaDes)
                            If Not operazioniNoMagazzino.Contains(currentAttivitaDes) Then
                                operazioniNoMagazzino.Add(currentAttivitaDes)
                            End If
                    End Select


                Case enum_ModificaMultiplaOperazioni.ModificaxModificaSupImpMantenendoQtaTotaleProdotto

                    If ModificaxModificaSupImpMantenendoQtaProdotto(Id_Agenda, Piva, currentAttivitaDes, errMess, objParametri_Server) Then
                        nOpModificate += 1
                    End If
                Case enum_ModificaMultiplaOperazioni.ModificaxModificaSupImpMantenendoQtaHaProdotto

                    If ModificaxModificaSupImpMantenendoQtaHaProdotto(Id_Agenda, Piva, currentAttivitaDes, errMess, objParametri_Server) Then
                        nOpModificate += 1
                    End If
            End Select
        Next

        generaMessaggioRisultato(nOpModificate, lista_Attivita.Count, errMess, magazziniGiaAssociati, operazioniNoMagazzino, lista_Errori)

    End Sub


    Private Function AggiungiMacchine(InfoOperazione As InfoOperazione,
                                      currentAttivitaDes As String,
                                      lista_Macchine As List(Of RisorsaMacchina),
                                      Id_Agenda As Integer,
                                      Piva As String,
                                      Sa_Cod As Integer,
                                      Data As Date,
                                      EliminaPrecedenti As Boolean,
                                      ByRef errMess As List(Of String),
                                      objParametri_Server As AgronicaCoreParametri
                                      ) As Boolean

        Dim objMovimento As New Movimenti_R
        Dim objDettagli_R As New Movimenti_Dettagli_R

        Dim DtMov, DtMovDet As DataTable
        Dim Id_Mov, Id_Mov_Det, Mac_Cod As Integer
        Dim Mac_Des As String

        Dim BDummy As Boolean
        Dim AlmenoUna As Boolean = False

        If EliminaPrecedenti = True Then
            If EliminaMacchine(Id_Agenda, Piva, currentAttivitaDes, errMess, objParametri_Server) Then
                AlmenoUna = True
            End If
        End If

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        For Each macchina In lista_Macchine
            'TRANSAZIONE SINGOLA PER OGNI ELEMENTO
            'Istanzio la transazione forzando l'uso di una nuova transazione
            AgronicaCoreDataProvider.Utility.VerificaApriTransazione(objParametri_Server, flagConnessione, flagTransazione)

            Id_Mov = 0
            Id_Mov_Det = 0

            Mac_Cod = macchina.macchina.codice
            Mac_Des = macchina.macchina.descrizione

            DtMov = objMovimento.Leggi(Piva, 0, Id_Agenda, 0, 0, CAU_IMPUTAZIONE_PARCOMACCHINE,
                                       AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "", "", objParametri_Server)

            'Se il movimento non esiste ancora lo creo
            If DtMov.Rows.Count = 0 Then
                Id_Mov = generaNuovoIdTabella("Movimenti", InfoOperazione, objParametri_Server)

                BDummy = scriviMovimento(Piva, Sa_Cod, Id_Agenda, Id_Mov,
                                         "Imputazione di Costi Dovuti ad Utilizzo del Parco Macchine",
                                         CAU_IMPUTAZIONE_PARCOMACCHINE, Data, objParametri_Server)

                If BDummy = False Then
                    Dim msg = "- " & String.Format(ImpossibileAggiungereMacchinaAttrezzaturaXOperazioneY, Mac_Des, currentAttivitaDes)
                    errMess.Add(msg)
                    Continue For
                End If
            Else
                Id_Mov = DtMov.Rows(0).Item("id_mov")
            End If

            DtMovDet = objDettagli_R.Leggi(Piva, Sa_Cod, Id_Agenda, Id_Mov,
                                           0, 0, 0, Mac_Cod,
                                           "", 0, 0, 0,
                                           0, 0, 0,
                                           AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "", "", objParametri_Server)

            If DtMovDet.Rows.Count = 0 Then
                Id_Mov_Det = generaNuovoIdTabella("Movimenti_Dettagli", InfoOperazione, objParametri_Server)


                Dim objDettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
                BDummy = scriviDettagli(Piva, Sa_Cod, Id_Agenda,
                                            Id_Mov, Id_Mov_Det,
                                            MACCHINE, Mac_Cod, objParametri_Server)

                If BDummy = True Then
                    AlmenoUna = True
                Else
                    'Return 2 'errore
                    Dim msg = "- " & String.Format(ImpossibileAggiungereMacchinaAttrezzaturaXOperazioneY, Mac_Des, currentAttivitaDes)
                    errMess.Add(msg)
                End If
            Else
                'Macchina già presente, non faccio nulla (o dico cmq di averla modificata?)
            End If

            If BDummy = False Then
                AgronicaCoreDataProvider.Utility.VerificaAnnullaTransazione(objParametri_Server, False)
            Else
                AgronicaCoreDataProvider.Utility.VerificaChiudiTransazione(objParametri_Server, flagTransazione)
            End If
            AgronicaCoreDataProvider.Utility.VerificaChiudiConnessione(objParametri_Server, flagConnessione)

        Next

        Return AlmenoUna

    End Function

    Private Function EliminaMacchine(Id_Agenda As Integer,
                                     Piva As String,
                                     currentAttivitaDes As String,
                                     ByRef errMess As List(Of String),
                                     objParametri_Server As AgronicaCoreParametri
                                     ) As Boolean


        Dim objMovimento As New Movimenti_R
        Dim Dt As DataTable

        Dim AlmenoUna As Boolean = False
        Dim err As Boolean = False

        Dt = objMovimento.Leggi(Piva, 0, Id_Agenda, 0,
                                0, CAU_IMPUTAZIONE_PARCOMACCHINE,
                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                "", "", objParametri_Server)

        For Each movimento As DataRow In Dt.Rows
            Try
                Dim objMC As New Agenda_Movimenti_Helper
                objMC.Cancella(Piva, 0,
                               Id_Agenda, movimento.Item("id_mov"),
                               objParametri_Server)

                AlmenoUna = True

            Catch ex As Exception
                err = True
            End Try
        Next

        If err Then
            errMess.Add("- " & String.Format(ImpossibileEliminareAlcuneMacchineAssociateOperazioneX, currentAttivitaDes))
        End If

        Return AlmenoUna
    End Function

    Private Function AggiungiContatti(InfoOperazione As InfoOperazione,
                                      currentAttivitaDes As String,
                                      lista_Persone As List(Of RisorsaPersona),
                                      Id_Agenda As Integer,
                                      Piva As String,
                                      Sa_Cod As Integer,
                                      Data As Date,
                                      EliminaPrecedenti As Boolean,
                                      ByRef errMess As List(Of String),
                                      objParametri_Server As AgronicaCoreParametri
                                      ) As Boolean

        Dim objMovimento As New Movimenti_R
        Dim objDettagli_R As New Movimenti_Dettagli_R

        Dim DtMov, DtMovDet As DataTable
        Dim Id_Mov, Id_Mov_Det,
            Cod_Risum, Cod_Rapp As Integer
        Dim Cau_Mov, Rag_Soc, Rapporto_Des As String

        Dim BDummy As Boolean
        Dim AlmenoUna As Boolean = False

        'Elimino tutti i movimenti se richiesto dall'utente
        If EliminaPrecedenti = True Then
            If EliminaContatti(Id_Agenda, Piva, Sa_Cod, currentAttivitaDes, errMess, objParametri_Server) Then
                AlmenoUna = True
            End If
        End If

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        For Each persona In lista_Persone
            'TRANSAZIONE SINGOLA PER OGNI ELEMENTO
            'Istanzio la transazione forzando l'uso di una nuova transazione
            AgronicaCoreDataProvider.Utility.VerificaApriTransazione(objParametri_Server, flagConnessione, flagTransazione)

            Id_Mov = 0
            Id_Mov_Det = 0
            Cau_Mov = ""

            Cod_Rapp = persona.risorsaUmana.rapportoContabile.codice
            Cod_Risum = persona.risorsaUmana.codice
            Rag_Soc = persona.risorsaUmana.contatto.ragione_Sociale
            Rapporto_Des = persona.risorsaUmana.rapportoContabile.descrizione

            Select Case Cod_Rapp
                Case COD_LEGALE, COD_DIPENDENTE
                    Cau_Mov = CAU_IMPUTAZIONE_MANODOPERA
                Case COD_TERZISTA
                    'Manodopera
                    Cau_Mov = CAU_IMPUTAZIONE_TERZISTI
                Case COD_TECNICORESPONSABILE
                    'Manodopera
                    Cau_Mov = CAU_IMPUTAZIONE_TECNICO_RESPONSABILE
                Case Else
                    Cau_Mov = CAU_IMPUTAZIONE_MANODOPERA
            End Select

            DtMov = objMovimento.Leggi(Piva, 0, Id_Agenda, 0, 0, Cau_Mov,
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "", "", objParametri_Server)

            'Se il movimento non esiste ancora lo creo
            If DtMov.Rows.Count = 0 Then
                Id_Mov = generaNuovoIdTabella("Movimenti", InfoOperazione, objParametri_Server)

                BDummy = scriviMovimento(Piva, Sa_Cod, Id_Agenda, Id_Mov,
                                         "Imputazione di Costi Dovuti a Personale",
                                         Cau_Mov, Data, objParametri_Server)
                If BDummy = False Then
                    Dim msg = "- " & String.Format(ImpossibileAggiungereContattoXRapportoYOperazioneZ, Rag_Soc, Rapporto_Des, currentAttivitaDes)
                    errMess.Add(msg)
                    Continue For
                End If

            Else
                Id_Mov = DtMov.Rows(0).Item("id_mov")
            End If

            DtMovDet = objDettagli_R.Leggi(Piva, Sa_Cod, Id_Agenda, Id_Mov,
                                           0, 0, 0, Cod_Risum,
                                           Cau_Mov, 0, 0, 0, 0,
                                           0, 0,
                                           AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "", "", objParametri_Server)

            If DtMovDet.Rows.Count = 0 Then
                Id_Mov_Det = generaNuovoIdTabella("Movimenti_Dettagli", InfoOperazione, objParametri_Server)

                Dim objDettagli_W As New Movimenti_Dettagli_W
                BDummy = scriviDettagli(Piva, Sa_Cod, Id_Agenda, Id_Mov, Id_Mov_Det,
                                        ELEMCOD_MANODOPERA, Cod_Risum, objParametri_Server)

                If BDummy = True Then
                    AlmenoUna = True
                Else
                    'Return 2 'errore
                    Dim msg = "- " & String.Format(ImpossibileAggiungereContattoXRapportoYOperazioneZ, Rag_Soc, Rapporto_Des, currentAttivitaDes)
                    errMess.Add(msg)
                End If
            Else
                'Contatto già presente, non faccio nulla (o dico cmq di averla modificata?)
            End If

            If BDummy = False Then
                AgronicaCoreDataProvider.Utility.VerificaAnnullaTransazione(objParametri_Server, False)
            Else
                AgronicaCoreDataProvider.Utility.VerificaChiudiTransazione(objParametri_Server, flagTransazione)
            End If
            AgronicaCoreDataProvider.Utility.VerificaChiudiConnessione(objParametri_Server, flagConnessione)

        Next

        Return AlmenoUna

    End Function

    Private Function EliminaContatti(Id_Agenda As Integer,
                                     Piva As String,
                                     Sa_Cod As Integer,
                                     currentAttivitaDes As String,
                                     ByRef errMess As List(Of String),
                                     objParametri_Server As AgronicaCoreParametri
                                     ) As Boolean

        Dim objMovimento As New Movimenti_R
        Dim Dt As DataTable

        Dim AlmenoUna As Boolean = False
        Dim err As Boolean = False

        Dt = objMovimento.Leggi(Piva, Sa_Cod, Id_Agenda, 0, 0, "",
                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                " Movimenti.Cau_Mov IN ('" & CAU_IMPUTAZIONE_MANODOPERA & "','" & CAU_IMPUTAZIONE_TERZISTI & "','" & CAU_IMPUTAZIONE_TECNICO_RESPONSABILE & "') ",
                                "", objParametri_Server)


        For Each movimento As DataRow In Dt.Rows
            Try
                Dim objMC As New Agenda_Movimenti_Helper
                objMC.Cancella(Piva, 0,
                               Id_Agenda, movimento.Item("id_mov"),
                               objParametri_Server)

                AlmenoUna = True
            Catch ex As Exception
                err = True
            End Try
        Next

        If err Then
            errMess.Add("- " & String.Format(ImpossibileEliminareAlcuniContattiAssociatiOperazioneX, currentAttivitaDes))
        End If

        Return AlmenoUna
    End Function

    Private Function AggiungiMagazzino(BLOCCA_SE_SUPERA_GIACENZE As Boolean,
                                       Id_Agenda As Integer,
                                       Piva As String,
                                       Fabbricato_Cod As Integer,
                                       Piva_Fabbricato As String,
                                       Sa_Cod_Fabbricato As Integer,
                                       ByRef errMess As List(Of String),
                                       ByRef magazziniGiaAssociati As List(Of String),
                                       objParametri_Server As AgronicaCoreParametri
                                       ) As Boolean

        Dim DoseTotale, N_Inneschi As Decimal
        Dim Cod_Innesco, Mezzo As Integer

        Dim Movimento As Movimento
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione

        Dim objAgenda As New Agenda_Operazione_Helper
        Dim Agenda As New Operazione_Agenda
        Agenda = objAgenda.Leggi(Piva, 0, CInt(Id_Agenda), 0, objParametri_Server)

        If Not IsNothing(Agenda) Then

            If Not IsNothing(Agenda.Movimenti) Then

                Dim Lista_Dettagli As New List(Of Movimento_Dettaglio)

                '------------------------
                'MOVIMENTO LAVORAZIONE
                '------------------------
                'Modifico le destinazioni --> Qta e Qta2 
                For Each Movimento In Agenda.Movimenti

                    Select Case Movimento.Cau_Mov

                        Case CAU_TRATTAMENTO, CAU_LAVORAZIONE, CAU_RILIEVO_CAMPO

                            Mezzo = Movimento.Mezzo

                            '------------------------
                            'MOVIMENTI_DETTAGLI
                            '------------------------
                            If Not IsNothing(Movimento.Movimenti_Dettagli) Then

                                Select Case Agenda.Lav_Cod

                                    '1 dettaglio per ogni impianto
                                    '1 dettaglio tecnico per ogni trappola contenente i dati dell'innesco
                                    Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                                        '------------------------
                                        'DETTAGLIO TRAPPOLA SCARICO
                                        '------------------------
                                        Dim Movimento_Dettaglio_Trappola As New Movimento_Dettaglio

                                        Movimento_Dettaglio_Trappola.Id_Agenda = Agenda.Id_Agenda
                                        Movimento_Dettaglio_Trappola.Piva = Agenda.Piva
                                        Movimento_Dettaglio_Trappola.Sa_Cod = Sa_Cod_Fabbricato
                                        Movimento_Dettaglio_Trappola.Data = Agenda.Data
                                        Movimento_Dettaglio_Trappola.Lav_Cod = Agenda.Lav_Cod
                                        Movimento_Dettaglio_Trappola.Cau_Mov = CAU_SCARICO

                                        N_Inneschi = 0
                                        Cod_Innesco = 0

                                        For Each dettaglio In Movimento.Movimenti_Dettagli
                                            Dim j = 0
                                            'Prendo i dati dal primo dettaglio (Qta inclusa)
                                            If j = 0 Then

                                                Movimento_Dettaglio_Trappola.Elem_Cod = dettaglio.Elem_Cod
                                                Movimento_Dettaglio_Trappola.Pro_Cod = dettaglio.Pro_Cod
                                                Movimento_Dettaglio_Trappola.Mat_Cod = dettaglio.Mat_Cod

                                                Movimento_Dettaglio_Trappola.Extra_Int = 0
                                                Movimento_Dettaglio_Trappola.Udm_Cod = dettaglio.Udm_Cod
                                                Movimento_Dettaglio_Trappola.Qta = dettaglio.Qta

                                                Movimento_Dettaglio_Trappola.Contabilizzato = dettaglio.Contabilizzato

                                                j += 1
                                            End If

                                            'Movimenti_Dettagli_Tecnici (somma totale inneschi e codice)
                                            If Not IsNothing(dettaglio.Movimenti_Dettagli_Tecnici) Then
                                                For x = 0 To dettaglio.Movimenti_Dettagli_Tecnici.Count - 1
                                                    N_Inneschi += dettaglio.Movimenti_Dettagli_Tecnici(x).Dose
                                                    Cod_Innesco = dettaglio.Movimenti_Dettagli_Tecnici(x).Av_Cod
                                                Next
                                            End If
                                        Next

                                        Movimento_Dettaglio_Trappola.BaseCode = Agenda.BaseCode
                                        Movimento_Dettaglio_Trappola.TopCode = Agenda.TopCode

                                        '------------------------------------------------
                                        'MOVIMENTO DESTINAZIONE SCARICO TRAPPOLA
                                        '------------------------------------------------
                                        Dim Lista_Destinazioni_Trappola As New List(Of Movimento_Destinazione)

                                        Dim Movimento_Destinazione_Trappola = New Movimento_Destinazione

                                        Movimento_Destinazione_Trappola.Data = Agenda.Data

                                        Movimento_Destinazione_Trappola.Id_Agenda = Agenda.Id_Agenda
                                        Movimento_Destinazione_Trappola.Piva = Piva_Fabbricato
                                        Movimento_Destinazione_Trappola.Sa_Cod = Sa_Cod_Fabbricato
                                        Movimento_Destinazione_Trappola.Appezza = 0
                                        Movimento_Destinazione_Trappola.Id_Destinazione = Fabbricato_Cod
                                        Movimento_Destinazione_Trappola.Tipo = MAGAZZINO

                                        Movimento_Destinazione_Trappola.Qta = Movimento_Dettaglio_Trappola.Qta

                                        Movimento_Destinazione_Trappola.BaseCode = Agenda.BaseCode
                                        Movimento_Destinazione_Trappola.TopCode = Agenda.TopCode

                                        Lista_Destinazioni_Trappola.Add(Movimento_Destinazione_Trappola)

                                        Movimento_Dettaglio_Trappola.Movimenti_Destinazioni = Lista_Destinazioni_Trappola

                                        Lista_Dettagli.Add(Movimento_Dettaglio_Trappola)

                                        '------------------------
                                        'DETTAGLIO INNESCHI
                                        '------------------------
                                        '------------------------
                                        'DETTAGLIO TRAPPOLA SCARICO
                                        '------------------------
                                        Dim Movimento_Dettaglio_Innesco = New Movimento_Dettaglio

                                        Movimento_Dettaglio_Innesco.Id_Agenda = Agenda.Id_Agenda
                                        Movimento_Dettaglio_Innesco.Piva = Agenda.Piva
                                        Movimento_Dettaglio_Innesco.Sa_Cod = Sa_Cod_Fabbricato
                                        Movimento_Dettaglio_Innesco.Data = Agenda.Data
                                        Movimento_Dettaglio_Innesco.Lav_Cod = Agenda.Lav_Cod
                                        Movimento_Dettaglio_Innesco.Cau_Mov = CAU_SCARICO

                                        Movimento_Dettaglio_Innesco.Elem_Cod = INNESCHI
                                        Movimento_Dettaglio_Innesco.Pro_Cod = Cod_Innesco
                                        Movimento_Dettaglio_Innesco.Mat_Cod = 0

                                        Movimento_Dettaglio_Innesco.Extra_Int = 0
                                        Movimento_Dettaglio_Innesco.Udm_Cod = enum_UnitaMisura.Numero_Inneschi
                                        Movimento_Dettaglio_Innesco.Qta = N_Inneschi

                                        Movimento_Dettaglio_Innesco.Contabilizzato = Movimento_Dettaglio_Trappola.Contabilizzato

                                        Movimento_Dettaglio_Innesco.BaseCode = Agenda.BaseCode
                                        Movimento_Dettaglio_Innesco.TopCode = Agenda.TopCode

                                        '------------------------------------------------
                                        'MOVIMENTO DESTINAZIONE SCARICO TRAPPOLA
                                        '------------------------------------------------
                                        Dim Lista_Destinazioni_Innesco As New List(Of Movimento_Destinazione)

                                        Dim Movimento_Destinazione_Innesco = New Movimento_Destinazione

                                        Movimento_Destinazione_Innesco.Data = Agenda.Data

                                        Movimento_Destinazione_Innesco.Id_Agenda = Agenda.Id_Agenda
                                        Movimento_Destinazione_Innesco.Piva = Piva_Fabbricato
                                        Movimento_Destinazione_Innesco.Sa_Cod = Sa_Cod_Fabbricato
                                        Movimento_Destinazione_Innesco.Appezza = 0
                                        Movimento_Destinazione_Innesco.Id_Destinazione = Fabbricato_Cod
                                        Movimento_Destinazione_Innesco.Tipo = MAGAZZINO

                                        Movimento_Destinazione_Innesco.Qta = N_Inneschi

                                        Movimento_Destinazione_Innesco.BaseCode = Agenda.BaseCode
                                        Movimento_Destinazione_Innesco.TopCode = Agenda.TopCode

                                        Lista_Destinazioni_Innesco.Add(Movimento_Destinazione_Innesco)

                                        Movimento_Dettaglio_Innesco.Movimenti_Destinazioni = Lista_Destinazioni_Innesco

                                        Lista_Dettagli.Add(Movimento_Dettaglio_Innesco)


                                        '1 dettaglio per ogni trappola
                                        '1 dettaglio tecnico per ogni trappola contenente i dati dell'innesco
                                    Case LAVCOD_REINNESCO_TRAPPOLE

                                        For Each dettaglio In Movimento.Movimenti_Dettagli

                                            Movimento_Dettaglio = New Movimento_Dettaglio

                                            Movimento_Dettaglio.Id_Agenda = Agenda.Id_Agenda
                                            Movimento_Dettaglio.Piva = Agenda.Piva
                                            Movimento_Dettaglio.Sa_Cod = Sa_Cod_Fabbricato
                                            Movimento_Dettaglio.Data = Agenda.Data
                                            Movimento_Dettaglio.Lav_Cod = Agenda.Lav_Cod
                                            Movimento_Dettaglio.Cau_Mov = CAU_SCARICO

                                            N_Inneschi = 0
                                            Cod_Innesco = 0

                                            'Movimenti_Dettagli_Tecnici (num inneschi e codice)
                                            If Not IsNothing(dettaglio.Movimenti_Dettagli_Tecnici) Then
                                                For Each dettaglio_tecnico In dettaglio.Movimenti_Dettagli_Tecnici
                                                    N_Inneschi = dettaglio_tecnico.Dose
                                                    Cod_Innesco = dettaglio_tecnico.Av_Cod
                                                Next
                                            End If

                                            Movimento_Dettaglio.Elem_Cod = INNESCHI
                                            Movimento_Dettaglio.Pro_Cod = Cod_Innesco
                                            Movimento_Dettaglio.Mat_Cod = 0
                                            Movimento_Dettaglio.Qta = N_Inneschi
                                            Movimento_Dettaglio.Udm_Cod = enum_UnitaMisura.Numero_Inneschi

                                            Movimento_Dettaglio.Contabilizzato = dettaglio.Contabilizzato

                                            Movimento_Dettaglio.BaseCode = Agenda.BaseCode
                                            Movimento_Dettaglio.TopCode = Agenda.TopCode

                                            '------------------------
                                            'MOVIMENTO DESTINAZIONE SCARICO
                                            '------------------------
                                            Dim Lista_Destinazioni As New List(Of Movimento_Destinazione)
                                            Movimento_Destinazione = New Movimento_Destinazione

                                            Movimento_Destinazione.Data = Agenda.Data

                                            Movimento_Destinazione.Id_Agenda = Agenda.Id_Agenda
                                            Movimento_Destinazione.Piva = Piva_Fabbricato
                                            Movimento_Destinazione.Sa_Cod = Sa_Cod_Fabbricato
                                            Movimento_Destinazione.Appezza = 0
                                            Movimento_Destinazione.Id_Destinazione = Fabbricato_Cod
                                            Movimento_Destinazione.Tipo = MAGAZZINO

                                            Movimento_Destinazione.Qta = N_Inneschi

                                            Movimento_Destinazione.BaseCode = Agenda.BaseCode
                                            Movimento_Destinazione.TopCode = Agenda.TopCode

                                            Lista_Destinazioni.Add(Movimento_Destinazione)

                                            Movimento_Dettaglio.Movimenti_Destinazioni = Lista_Destinazioni

                                            Lista_Dettagli.Add(Movimento_Dettaglio)
                                        Next

                                    Case Else
                                        '------------------------
                                        'TUTTE LE ALTRE OPERAZIONI
                                        '------------------------
                                        '1 dettaglio per ogni prodotto

                                        For Each dettaglio In Movimento.Movimenti_Dettagli
                                            Movimento_Dettaglio = New Movimento_Dettaglio

                                            Movimento_Dettaglio.Id_Agenda = Agenda.Id_Agenda
                                            Movimento_Dettaglio.Piva = Agenda.Piva
                                            Movimento_Dettaglio.Sa_Cod = Sa_Cod_Fabbricato
                                            Movimento_Dettaglio.Data = Agenda.Data
                                            Movimento_Dettaglio.Lav_Cod = Agenda.Lav_Cod
                                            Movimento_Dettaglio.Cau_Mov = CAU_SCARICO

                                            Movimento_Dettaglio.Elem_Cod = dettaglio.Elem_Cod
                                            Movimento_Dettaglio.Pro_Cod = dettaglio.Pro_Cod
                                            Movimento_Dettaglio.Mat_Cod = dettaglio.Mat_Cod

                                            Movimento_Dettaglio.Extra_Int = 0
                                            Movimento_Dettaglio.Udm_Cod = dettaglio.Udm_Cod

                                            Movimento_Dettaglio.Contabilizzato = dettaglio.Contabilizzato

                                            DoseTotale = 0

                                            '------------------------
                                            'MOVIMENTI_DESTINAZIONI (somma totale prodotto)
                                            '------------------------
                                            If Not IsNothing(dettaglio.Movimenti_Destinazioni) Then
                                                For Each destinazione In dettaglio.Movimenti_Destinazioni
                                                    DoseTotale += destinazione.Qta
                                                Next
                                            End If

                                            Movimento_Dettaglio.Qta = Math.Round(DoseTotale, 4)

                                            Movimento_Dettaglio.BaseCode = Agenda.BaseCode
                                            Movimento_Dettaglio.TopCode = Agenda.TopCode

                                            '------------------------
                                            'MOVIMENTO DESTINAZIONE SCARICO
                                            '------------------------
                                            Dim Lista_Destinazioni As New List(Of Movimento_Destinazione)

                                            Movimento_Destinazione = New Movimento_Destinazione

                                            Movimento_Destinazione.Data = Agenda.Data

                                            Movimento_Destinazione.Id_Agenda = Agenda.Id_Agenda
                                            Movimento_Destinazione.Piva = Piva_Fabbricato
                                            Movimento_Destinazione.Sa_Cod = Sa_Cod_Fabbricato
                                            Movimento_Destinazione.Appezza = 0
                                            Movimento_Destinazione.Id_Destinazione = Fabbricato_Cod
                                            Movimento_Destinazione.Tipo = MAGAZZINO

                                            Movimento_Destinazione.Qta = DoseTotale

                                            Movimento_Destinazione.BaseCode = Agenda.BaseCode
                                            Movimento_Destinazione.TopCode = Agenda.TopCode

                                            Lista_Destinazioni.Add(Movimento_Destinazione)

                                            Movimento_Dettaglio.Movimenti_Destinazioni = Lista_Destinazioni

                                            Lista_Dettagli.Add(Movimento_Dettaglio)
                                        Next
                                End Select
                            End If

                            'Se l'operazione ha già il magazzino non faccio nulla
                        Case CAU_SCARICO
                            magazziniGiaAssociati.Add(Agenda.Des_Lib & " (" & Agenda.Data.ToShortDateString() & ")")
                            Return False
                    End Select
                Next

                'Se ho recuperato tutti i dettagli
                'aggiungo movimento scarico
                Dim qta_in_data As Decimal
                Dim Qta As Decimal

                If Not Lista_Dettagli Is Nothing AndAlso Lista_Dettagli.Count > 0 Then

                    'Se l'utente ha il blocco verifico le Qta in giacenza
                    If BLOCCA_SE_SUPERA_GIACENZE = True Then

                        For Each dettaglio In Lista_Dettagli
                            Qta = dettaglio.Qta

                            'Guardo se la quantità è conforme per la data di intervento
                            Dim objMovDet As New Movimenti_Dettagli_R
                            qta_in_data = objMovDet.Verifica_Giacenze_Con_Magazzino_Esterno(Piva_Fabbricato, Sa_Cod_Fabbricato, Fabbricato_Cod,
                                                                                            dettaglio.Elem_Cod,
                                                                                            dettaglio.Pro_Cod,
                                                                                            dettaglio.Mat_Cod,
                                                                                            0, 0, LOTTO_NONDEFINITO, 0,
                                                                                            dettaglio.Udm_Cod,
                                                                                            AGRODATAINIZIO, Agenda.Data,
                                                                                            Agenda.Piva, Agenda.Sa_Cod, Agenda.Id_Agenda,
                                                                                            objParametri_Server)
                            If Qta > Math.Round(qta_in_data, 4) Then
                                Dim UDM_Des As String = GetDescrizioneUDM(dettaglio.Udm_Cod, objParametri_Server)
                                errMess.Add("- " & String.Format(ImpossibileAssociareMagazzinoOperazioneXQuantitàProdottoPresenteQTAUdmDesNonSufficienteScaricoQTAUdmDes, Agenda.Des_Lib, Math.Round(qta_in_data, 4).ToString(), UDM_Des, Math.Round(Qta, 4).ToString(), UDM_Des))
                                Return False
                            End If
                        Next
                    End If

                    'Inserimento: MOVIMENTO - DETTAGLI - DESTINAZIONI
                    Try

                        Movimento = New Movimento

                        Movimento.Id_Agenda = Agenda.Id_Agenda
                        Movimento.Piva = Piva_Fabbricato
                        Movimento.Sa_Cod = Sa_Cod_Fabbricato
                        Movimento.Data = Agenda.Data
                        Movimento.Lav_Cod = Agenda.Lav_Cod

                        Movimento.Cau_Mov = CAU_SCARICO
                        Movimento.Mov_Desc = "Scarico Magazzino"
                        Movimento.Mezzo = Mezzo

                        Movimento.BaseCode = Agenda.BaseCode
                        Movimento.TopCode = Agenda.TopCode

                        Movimento.Movimenti_Dettagli = Lista_Dettagli

                        Dim ObjMovimento As New Agenda_Movimenti_Helper
                        ObjMovimento.Scrivi(Movimento, objParametri_Server)
                        ObjMovimento = Nothing

                    Catch ex As Exception
                        Throw ex
                    End Try
                End If
            End If
        End If

        Return True

    End Function

    Private Function ModificaxModificaSupImpMantenendoQtaProdotto(Id_Agenda As Integer,
                                                                  Piva As String,
                                                                  currentAttivitaDes As String,
                                                                  ByRef errMess As List(Of String),
                                                                  objParametri_Server As AgronicaCoreParametri
                                                                  ) As Boolean

        Dim Lav_Cod As Integer = 0
        Dim Fr_Cod As Integer = 0
        Dim Mat_Cod As Integer = 0
        Dim Udm_Cod As Integer = 0
        Dim Udm_Cod_Trasformato As Integer = 0
        Dim Dett_Cod As Integer = 0
        Dim Dose_Old As Decimal = 0
        Dim Dose_Tot_Old As Decimal = 0

        Dim Qta_Dest_New As Decimal = 0
        Dim Dose_New As Decimal = 0
        Dim Dose_New_Hl As Decimal = 0
        Dim Sup_Tot_Old As Decimal = 0
        Dim Sup_Tot_New As Decimal = 0
        Dim Dose_Trasformata_New As Decimal = 0

        Dim AcquaTot As Decimal = 0

        Dim strFiltroImp As String = ""

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False


        Dim objAgenda As New Agenda_Operazione_Helper

        Dim Agenda = objAgenda.Leggi(Piva, 0,
                                     CInt(Id_Agenda),
                                     0,
                                     objParametri_Server)

        Try
            AgronicaCoreDataProvider.Utility.VerificaApriTransazione(objParametri_Server, flagConnessione, flagTransazione)

            If Not IsNothing(Agenda) Then
                Lav_Cod = Agenda.Lav_Cod

                'MOVIMENTI
                If Not IsNothing(Agenda.Movimenti) Then
                    Dim lista_Impianti As New List(Of EsercizioCDC)

                    For Each movimento In Agenda.Movimenti
                        Select Case movimento.Cau_Mov

                            Case CAU_TRATTAMENTO, CAU_LAVORAZIONE, CAU_RILIEVO_RACCOLTA
                                'ricavo subito acqua, destinazioni e sup_totale
                                If Sup_Tot_Old = 0 Then

                                    Select Case Agenda.Lav_Cod

                                    '1 destinazione per ogni dettaglio
                                        Case LAVCOD_CATTURE_MASSA, LAVCOD_INSTALLAZIONE_TRAPPOLE

                                            If Not IsNothing(movimento.Movimenti_Dettagli) Then

                                                For Each mov_dett As Movimento_Dettaglio In movimento.Movimenti_Dettagli

                                                    If Not IsNothing(mov_dett.Movimenti_Destinazioni) Then

                                                        For Each mov_dest As Movimento_Destinazione In mov_dett.Movimenti_Destinazioni
                                                            Sup_Tot_Old += CDec(mov_dest.Qta2)

                                                            Dim appezzamentoPK = New Appezzamento.PK(mov_dest.Appezza, New CentroAziendale.PK(mov_dest.Sa_Cod, mov_dest.Piva))
                                                            Dim impiantoPK = New Impianto.PK(mov_dest.Id_Destinazione, appezzamentoPK)

                                                            Dim esercizio = New Esercizio(0, "") With {
                                                                .impiantoPK = impiantoPK
                                                            }

                                                            Dim esercizioCDC As New EsercizioCDC With {
                                                                .superficieTrattata = mov_dest.Qta2,
                                                                .esercizio = esercizio
                                                            }

                                                            lista_Impianti.Add(esercizioCDC)

                                                            strFiltroImp &= " (Reg_Impianti.Piva='" & mov_dest.Piva & "' " &
                                                                    " AND Reg_Impianti.Sa_Cod=" & mov_dest.Sa_Cod & " " &
                                                                    " AND Reg_Impianti.appezza=" & mov_dest.Appezza & " " &
                                                                    " AND Reg_Impianti.id_reg=" & mov_dest.Id_Destinazione & " " &
                                                                    " ) OR "

                                                        Next
                                                    End If
                                                Next

                                                If strFiltroImp <> "" Then

                                                    Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                                    Dim DtImp As New DataTable
                                                    DtImp = objImp.Leggi(Piva, 0, 0, 0,
                                                                         enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                         " (" & Left(strFiltroImp, strFiltroImp.Length - 3) & ") ",
                                                                         "", objParametri_Server)

                                                    For Each impianto In lista_Impianti
                                                        Dim _piva As String = impianto.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                                                        Dim _sa_cod As Integer = impianto.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                                                        Dim _appezza As Integer = impianto.esercizio.impiantoPK.appezzamentoPK.codice
                                                        Dim _id_reg As Integer = impianto.esercizio.impiantoPK.codice
                                                        For Each p In DtImp.Rows
                                                            If _piva = p.Item("piva") And _sa_cod = p.Item("sa_cod") And _appezza = p.Item("appezza") And _id_reg = p.Item("id_reg") Then

                                                                impianto.esercizio.superficie = p.Item("sup_imp")

                                                                Sup_Tot_New += impianto.esercizio.superficie

                                                                Exit For
                                                            End If
                                                        Next
                                                    Next
                                                End If
                                            End If


                                            'n destinazioni per ogni dettaglio
                                        Case Else

                                            If Not IsNothing(movimento.Movimenti_Dettagli) AndAlso Not IsNothing(movimento.Movimenti_Dettagli(0).Movimenti_Destinazioni) Then

                                                For Each mov_dest As Movimento_Destinazione In movimento.Movimenti_Dettagli(0).Movimenti_Destinazioni

                                                    Sup_Tot_Old += CDec(mov_dest.Qta2)

                                                    Dim appezzamentoPK = New Appezzamento.PK(mov_dest.Appezza, New CentroAziendale.PK(mov_dest.Sa_Cod, mov_dest.Piva))
                                                    Dim impiantoPK = New Impianto.PK(mov_dest.Id_Destinazione, appezzamentoPK)

                                                    Dim esercizio = New Esercizio(0, "") With {
                                                        .impiantoPK = impiantoPK
                                                    }

                                                    Dim esercizioCDC As New EsercizioCDC With {
                                                        .superficieTrattata = mov_dest.Qta2,
                                                        .esercizio = esercizio
                                                    }

                                                    lista_Impianti.Add(esercizioCDC)

                                                    strFiltroImp &= " (Reg_Impianti.Piva='" & mov_dest.Piva & "' " &
                                                            " AND Reg_Impianti.Sa_Cod=" & mov_dest.Sa_Cod & " " &
                                                            " AND Reg_Impianti.appezza=" & mov_dest.Appezza & " " &
                                                            " AND Reg_Impianti.id_reg=" & mov_dest.Id_Destinazione & " " &
                                                            " ) OR "

                                                Next

                                                If strFiltroImp <> "" Then

                                                    Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                                    Dim DtImp As New DataTable
                                                    DtImp = objImp.Leggi(Piva, 0, 0, 0,
                                                                         enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                         " (" & Left(strFiltroImp, strFiltroImp.Length - 3) & ") ",
                                                                         "", objParametri_Server)

                                                    For Each impianto In lista_Impianti
                                                        Dim _piva As String = impianto.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                                                        Dim _sa_cod As Integer = impianto.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                                                        Dim _appezza As Integer = impianto.esercizio.impiantoPK.appezzamentoPK.codice
                                                        Dim _id_reg As Integer = impianto.esercizio.impiantoPK.codice
                                                        For Each p In DtImp.Rows
                                                            If _piva = p.Item("piva") And _sa_cod = p.Item("sa_cod") And _appezza = p.Item("appezza") And _id_reg = p.Item("id_reg") Then

                                                                impianto.esercizio.superficie = p.Item("sup_imp")

                                                                Sup_Tot_New += impianto.esercizio.superficie

                                                                Exit For
                                                            End If
                                                        Next
                                                    Next
                                                End If
                                            End If
                                    End Select
                                End If

                                '---------------------------------------------------
                                '----- Acqua  
                                '---------------------------------------------------

                                For Each dettaglio_tecnico In movimento.Movimenti_Dettagli_Tecnici
                                    Select Case dettaglio_tecnico.Qta_Ril
                                        Case Is > 0 'dosaggio totale --> da dividere
                                            AcquaTot = dettaglio_tecnico.Qta_Ril
                                        Case Is < 0 'dosaggio/ha --> già ok
                                            AcquaTot = Math.Abs(dettaglio_tecnico.Qta_Ril) * Sup_Tot_Old
                                    End Select
                                Next


                                'MOVIMENTI_DETTAGLI
                                If Not IsNothing(movimento.Movimenti_Dettagli) Then
                                    For Each dettaglio In movimento.Movimenti_Dettagli
                                        Fr_Cod = dettaglio.Pro_Cod
                                        Udm_Cod = dettaglio.Extra_Int
                                        Udm_Cod_Trasformato = dettaglio.Udm_Cod

                                        Dose_Old = dettaglio.Qta

                                        'dose e udm dell'irrigazione sono nel mov_dettaglio_tecnico
                                        If Agenda.Lav_Cod = LAVCOD_IRRIGAZIONE Then
                                            If Not IsNothing(dettaglio.Movimenti_Dettagli_Tecnici) AndAlso dettaglio.Movimenti_Dettagli_Tecnici.Count > 0 Then
                                                Dose_Old = dettaglio.Movimenti_Dettagli_Tecnici(0).Qta_Ril
                                                Dett_Cod = dettaglio.Movimenti_Dettagli_Tecnici(0).dett_cod
                                            End If
                                        End If




                                        Dose_Tot_Old = Dose_Old * Sup_Tot_Old
                                        Dose_New_Hl = 0

                                        Select Case Lav_Cod
                                            Case LAVCOD_DISTRIBUZIONE_INSETTI
                                                Dose_New = Math.Round(Dose_Tot_Old / Sup_Tot_New, 0)
                                            Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                             LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                             LAVCOD_RACCOLTA,
                                             LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                                                'qta è quella totale distribuita
                                                Dose_New = Dose_Old
                                            Case LAVCOD_IRRIGAZIONE
                                                Dose_New = Dose_Old
                                            Case Else
                                                Dose_New = Dose_Tot_Old / Sup_Tot_New
                                        End Select

                                        Select Case Lav_Cod

                                            Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                             LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                             LAVCOD_RACCOLTA,
                                             LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA

                                            Case LAVCOD_IRRIGAZIONE

                                            Case Else

                                                'Dose_Tot_New = Dose * SupTrattata_Tot
                                                'Dose_Tot_Old = Dose * Sup_Tot_Old

                                                ''dose ha
                                                'Movimento_Dettaglio.Qta = Dose
                                                If AcquaTot <> 0 Then
                                                    Dose_New_Hl = Dose_New * Sup_Tot_New / AcquaTot
                                                End If


                                                'modifico la qta del dettaglio (dose_ha)
                                                Dim objDet As New Movimenti_Dettagli_W
                                                objDet.Modifica_Quantita(dettaglio.Piva, dettaglio.Id_Agenda,
                                                                         dettaglio.Id_Mov, dettaglio.Id_Mov_Det,
                                                                         0, 0, 0, 0,
                                                                         Dose_New, Dose_New_Hl, Dose_Tot_Old,
                                                                         0, 0, 0,
                                                                         "", "",
                                                                         objParametri_Server)
                                        End Select



                                        Dim objDest As New Mov_Destinazioni_W

                                        'MOVIMENTI_DESTINAZIONI
                                        If Not IsNothing(dettaglio.Movimenti_Destinazioni) Then

                                            For Each destinazione In dettaglio.Movimenti_Destinazioni
                                                For Each impianto In lista_Impianti
                                                    Dim _piva As String = impianto.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                                                    Dim _sa_cod As Integer = impianto.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                                                    Dim _appezza As Integer = impianto.esercizio.impiantoPK.appezzamentoPK.codice
                                                    Dim _id_reg As Integer = impianto.esercizio.impiantoPK.codice

                                                    If _piva = destinazione.Piva And _sa_cod = destinazione.Sa_Cod And _appezza = destinazione.Appezza And _id_reg = destinazione.Id_Destinazione Then

                                                        Select Case dettaglio.Extra_Int
                                                            Case 3  'g
                                                                Dose_Trasformata_New = Dose_New / 1000  'caso in cui ho i grammi
                                                            Case 2032 'mg
                                                                Dose_Trasformata_New = Dose_New / 1000000  'caso in cui ho i grammi
                                                            Case 4  'q
                                                                Dose_Trasformata_New = Dose_New * 100  'caso in cui ho i quintali
                                                            Case 304 't
                                                                Dose_Trasformata_New = Dose_New * 1000   'caso in cui ho le tonnelate
                                                            Case 101 'ml
                                                                Dose_Trasformata_New = Dose_New / 1000  'caso in cui ho i ml
                                                            Case 104 'cc
                                                                Dose_Trasformata_New = Dose_New / 100  'caso in cui ho i cc
                                                            Case 19 'Metri cubi
                                                                Dose_Trasformata_New = Dose_New * 1000
                                                            Case Else
                                                                Dose_Trasformata_New = Dose_New

                                                        End Select

                                                        Dim QuotaDistribuzioneNew As Decimal = 0
                                                        If Sup_Tot_New <> 0 Then
                                                            QuotaDistribuzioneNew = impianto.esercizio.superficie / Sup_Tot_New
                                                        End If

                                                        Select Case Lav_Cod

                                                            Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA

                                                                'modifico solo sup_trattata
                                                                objDest.Modifica_SupTrattata(destinazione.Piva, destinazione.Sa_Cod,
                                                                                             destinazione.Id_Agenda,
                                                                                             destinazione.Id_Mov, destinazione.Id_Mov_Det,
                                                                                             destinazione.Appezza, destinazione.Id_Destinazione,
                                                                                             impianto.esercizio.superficie, QuotaDistribuzioneNew,
                                                                                             "", objParametri_Server)
                                                            Case Else


                                                                Select Case Lav_Cod
                                                                    Case LAVCOD_DISTRIBUZIONE_INSETTI
                                                                        Qta_Dest_New = Math.Round(Dose_New * impianto.esercizio.superficie, 0)

                                                                    Case LAVCOD_DISORIENTAMENTO_SESSUALE, LAVCOD_CONFUSIONE_SESSUALE,
                                                                         LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                                                         LAVCOD_RACCOLTA

                                                                        Qta_Dest_New = Math.Round((Dose_New / Sup_Tot_New) * impianto.esercizio.superficie, 0)

                                                                    Case LAVCOD_IRRIGAZIONE
                                                                        Select Case Dett_Cod
                                                                            Case enum_UnitaMisura.Millimetri
                                                                                Dose_New = Dose_New * 10
                                                                            Case enum_UnitaMisura.METRI3__HA
                                                                                Dose_New = Dose_New * 1
                                                                            Case Else
                                                                                Dose_New = 0
                                                                        End Select
                                                                        Qta_Dest_New = Dose_New * impianto.esercizio.superficie
                                                                    Case Else
                                                                        Qta_Dest_New = Dose_Trasformata_New * impianto.esercizio.superficie
                                                                End Select


                                                                'modifico qta_destinazione e sup_trattata
                                                                objDest.Modifica_Quantita_e_SupTrattata(destinazione.Piva, destinazione.Sa_Cod,
                                                                                                        destinazione.Id_Agenda,
                                                                                                        destinazione.Id_Mov, destinazione.Id_Mov_Det,
                                                                                                        destinazione.Appezza, destinazione.Id_Destinazione,
                                                                                                        Qta_Dest_New, impianto.esercizio.superficie,
                                                                                                        QuotaDistribuzioneNew,
                                                                                                        "", objParametri_Server)

                                                        End Select
                                                        Exit For
                                                    End If
                                                Next
                                            Next
                                        End If
                                    Next
                                End If
                        End Select
                    Next
                End If
            End If

            Dim objAgendaW As New Agenda_W
            Dim Flag_UpdateFlagOK As Boolean

            Flag_UpdateFlagOK = objAgendaW.Agenda_Sblocca2(Piva, Id_Agenda, objParametri_Server.UtenteUsername, Date.Now, "", objParametri_Server)

        Catch ex As Exception

            AgronicaCoreDataProvider.Utility.VerificaAnnullaTransazione(objParametri_Server, False)

            errMess.Add("- " & String.Format(ModificaNonRiuscitaPerOperazioneX, currentAttivitaDes))
            Return False

        Finally
            AgronicaCoreDataProvider.Utility.VerificaChiudiTransazione(objParametri_Server, flagTransazione)
            AgronicaCoreDataProvider.Utility.VerificaChiudiConnessione(objParametri_Server, flagConnessione)
        End Try

        Return True

    End Function

    Private Function ModificaxModificaSupImpMantenendoQtaHaProdotto(Id_Agenda As Integer,
                                                                    Piva As String,
                                                                    currentAttivitaDes As String,
                                                                    ByRef errMess As List(Of String),
                                                                    objParametri_Server As AgronicaCoreParametri
                                                                    ) As Boolean

        Dim Lav_Cod As Integer = 0
        Dim Fr_Cod As Integer = 0
        Dim Mat_Cod As Integer = 0
        Dim Udm_Cod As Integer = 0
        Dim Dett_Cod As Integer = 0
        Dim Udm_Cod_Trasformato As Integer = 0
        Dim Dose As Decimal = 0
        Dim Dose_New As Decimal = 0
        Dim Dose_New_Hl As Decimal = 0
        Dim Dose_Tot_Old As Decimal = 0
        Dim Dose_Tot_New As Decimal = 0
        Dim Lotto As String = ""
        Dim Cod_Progetto As Integer = 0

        Dim HashTotNew As New Hashtable

        Dim Qta_Dest_New As Decimal = 0
        Dim Sup_Tot_Old As Decimal = 0
        Dim Sup_Tot_New As Decimal = 0
        Dim Dose_Trasformata_New As Decimal = 0
        Dim DoseTotaleTrasformata_New As Decimal = 0

        Dim AcquaTot As Decimal = 0

        Dim strFiltroImp As String = ""


        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False


        Dim objAgenda As New Agenda_Operazione_Helper
        Dim Agenda = objAgenda.Leggi(Piva, 0, CInt(Id_Agenda), 0, objParametri_Server)

        Try
            AgronicaCoreDataProvider.Utility.VerificaApriTransazione(objParametri_Server, flagConnessione, flagTransazione)

            If Not IsNothing(Agenda) Then
                Lav_Cod = Agenda.Lav_Cod

                'MOVIMENTI
                If Not IsNothing(Agenda.Movimenti) Then
                    Dim lista_Impianti As New List(Of EsercizioCDC)

                    'MOVIMENTO LAVORAZIONE
                    'Modifico le destinazioni --> Qta e Qta2 
                    For Each movimento In Agenda.Movimenti

                        Select Case movimento.Cau_Mov

                            Case CAU_TRATTAMENTO, CAU_LAVORAZIONE, CAU_RILIEVO_RACCOLTA

                                'Ricavo subito acqua, destinazioni e sup_totale
                                If Sup_Tot_Old = 0 Then

                                    Select Case Agenda.Lav_Cod

                                            '1 destinazione per ogni dettaglio
                                        Case LAVCOD_CATTURE_MASSA, LAVCOD_INSTALLAZIONE_TRAPPOLE

                                            If Not IsNothing(movimento.Movimenti_Dettagli) Then

                                                For Each mov_dett As Movimento_Dettaglio In movimento.Movimenti_Dettagli

                                                    If Not IsNothing(mov_dett.Movimenti_Destinazioni) Then

                                                        For Each mov_dest As Movimento_Destinazione In mov_dett.Movimenti_Destinazioni
                                                            Sup_Tot_Old += CDec(mov_dest.Qta2)

                                                            Dim appezzamentoPK = New Appezzamento.PK(mov_dest.Appezza, New CentroAziendale.PK(mov_dest.Sa_Cod, mov_dest.Piva))
                                                            Dim impiantoPK = New Impianto.PK(mov_dest.Id_Destinazione, appezzamentoPK)

                                                            Dim esercizio = New Esercizio(0, "") With {
                                                                .impiantoPK = impiantoPK
                                                            }

                                                            Dim esercizioCDC As New EsercizioCDC With {
                                                                .superficieTrattata = mov_dest.Qta2,
                                                                .esercizio = esercizio
                                                            }

                                                            lista_Impianti.Add(esercizioCDC)

                                                            strFiltroImp &= " (Reg_Impianti.Piva = '" & mov_dest.Piva & "' " &
                                                                        " AND Reg_Impianti.Sa_Cod = " & mov_dest.Sa_Cod & " " &
                                                                        " AND Reg_Impianti.appezza = " & mov_dest.Appezza & " " &
                                                                        " AND Reg_Impianti.id_reg= " & mov_dest.Id_Destinazione & " " &
                                                                        " ) OR "

                                                        Next
                                                    End If
                                                Next

                                                If strFiltroImp <> "" Then

                                                    Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                                    Dim DtImp As New DataTable
                                                    DtImp = objImp.Leggi(Piva, 0, 0, 0,
                                                                         enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                         " (" & Left(strFiltroImp, strFiltroImp.Length - 3) & ") ",
                                                                         "", objParametri_Server)

                                                    For Each impianto In lista_Impianti
                                                        Dim _piva As String = impianto.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                                                        Dim _sa_cod As Integer = impianto.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                                                        Dim _appezza As Integer = impianto.esercizio.impiantoPK.appezzamentoPK.codice
                                                        Dim _id_reg As Integer = impianto.esercizio.impiantoPK.codice

                                                        For Each p As DataRow In DtImp.Rows
                                                            If _piva = p.Item("piva") And _sa_cod = p.Item("sa_cod") And
                                                                _appezza = p.Item("appezza") And _id_reg = p.Item("id_reg") Then

                                                                impianto.esercizio.superficie = p.Item("sup_imp")

                                                                Sup_Tot_New += impianto.esercizio.superficie

                                                                Exit For
                                                            End If
                                                        Next
                                                    Next
                                                End If
                                            End If

                                            'n destinazioni per ogni dettaglio
                                        Case Else

                                            If Not IsNothing(movimento.Movimenti_Dettagli) AndAlso Not IsNothing(movimento.Movimenti_Dettagli(0).Movimenti_Destinazioni) Then

                                                For Each mov_dest As Movimento_Destinazione In movimento.Movimenti_Dettagli(0).Movimenti_Destinazioni

                                                    Sup_Tot_Old += CDec(mov_dest.Qta2)

                                                    Dim appezzamentoPK = New Appezzamento.PK(mov_dest.Appezza, New CentroAziendale.PK(mov_dest.Sa_Cod, mov_dest.Piva))
                                                    Dim impiantoPK = New Impianto.PK(mov_dest.Id_Destinazione, appezzamentoPK)

                                                    Dim esercizio = New Esercizio(0, "") With {
                                                        .impiantoPK = impiantoPK
                                                    }

                                                    Dim esercizioCDC As New EsercizioCDC With {
                                                        .superficieTrattata = mov_dest.Qta2,
                                                        .esercizio = esercizio
                                                    }

                                                    lista_Impianti.Add(esercizioCDC)

                                                    strFiltroImp &= " (Reg_Impianti.Piva = '" & mov_dest.Piva & "' " &
                                                                " AND Reg_Impianti.Sa_Cod = " & mov_dest.Sa_Cod & " " &
                                                                " AND Reg_Impianti.appezza = " & mov_dest.Appezza & " " &
                                                                " AND Reg_Impianti.id_reg = " & mov_dest.Id_Destinazione & " " &
                                                                " ) OR "
                                                Next

                                                If strFiltroImp <> "" Then

                                                    Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                                    Dim DtImp As New DataTable
                                                    DtImp = objImp.Leggi(Piva, 0, 0, 0,
                                                                         enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                         " (" & Left(strFiltroImp, strFiltroImp.Length - 3) & ") ",
                                                                         "", objParametri_Server)

                                                    For Each impianto In lista_Impianti
                                                        Dim _piva As String = impianto.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                                                        Dim _sa_cod As Integer = impianto.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                                                        Dim _appezza As Integer = impianto.esercizio.impiantoPK.appezzamentoPK.codice
                                                        Dim _id_reg As Integer = impianto.esercizio.impiantoPK.codice

                                                        For Each p As DataRow In DtImp.Rows
                                                            If _piva = p.Item("piva") And _sa_cod = p.Item("sa_cod") And
                                                                _appezza = p.Item("appezza") And _id_reg = p.Item("id_reg") Then

                                                                impianto.esercizio.superficie = p.Item("sup_imp")

                                                                Sup_Tot_New += impianto.esercizio.superficie

                                                                Exit For
                                                            End If
                                                        Next
                                                    Next
                                                End If
                                            End If
                                    End Select
                                End If



                                '---------------------------------------------------
                                '----- Acqua  
                                '---------------------------------------------------
                                For Each dettaglio_tecnico In movimento.Movimenti_Dettagli_Tecnici
                                    Select Case dettaglio_tecnico.Qta_Ril
                                        Case Is > 0 'dosaggio totale --> da dividere
                                            AcquaTot = dettaglio_tecnico.Qta_Ril
                                        Case Is < 0 'dosaggio/ha --> già ok
                                            AcquaTot = Math.Abs(dettaglio_tecnico.Qta_Ril) * Sup_Tot_Old
                                    End Select
                                Next

                                'MOVIMENTI_DETTAGLI
                                If Not IsNothing(movimento.Movimenti_Dettagli) Then

                                    For Each dettaglio In movimento.Movimenti_Dettagli
                                        Fr_Cod = dettaglio.Pro_Cod
                                        Mat_Cod = dettaglio.Mat_Cod '(per semine e raccolte)
                                        Lotto = dettaglio.Lotto '(per semine)
                                        Udm_Cod = dettaglio.Extra_Int
                                        Udm_Cod_Trasformato = dettaglio.Udm_Cod
                                        Cod_Progetto = dettaglio.Cod_Progetto '(per semilavorati raccolta)

                                        Dose = dettaglio.Qta

                                        'dose e udm dell'irrigazione sono nel mov_dettaglio_tecnico
                                        If Agenda.Lav_Cod = LAVCOD_IRRIGAZIONE Then
                                            If Not IsNothing(dettaglio.Movimenti_Dettagli_Tecnici) AndAlso dettaglio.Movimenti_Dettagli_Tecnici.Count > 0 Then
                                                Dose = dettaglio.Movimenti_Dettagli_Tecnici(0).Qta_Ril
                                                Dett_Cod = dettaglio.Movimenti_Dettagli_Tecnici(0).dett_cod
                                            End If
                                        End If

                                        Select Case Lav_Cod
                                            Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                                 LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                                 LAVCOD_RACCOLTA,
                                                 LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                                                Dose_Tot_New = Math.Round(Dose / Sup_Tot_Old * Sup_Tot_New, 0)
                                                Dose_Tot_Old = Dose
                                            Case Else
                                                Dose_Tot_New = Dose * Sup_Tot_New
                                                Dose_Tot_Old = Dose * Sup_Tot_Old
                                        End Select

                                        Select Case dettaglio.Extra_Int
                                            Case enum_UnitaMisura.Grammi  'g
                                                Dose_Trasformata_New = Dose / 1000
                                                DoseTotaleTrasformata_New = Dose_Tot_New / 1000
                                            Case enum_UnitaMisura.Milligrammi
                                                Dose_Trasformata_New = Dose / 1000000
                                                DoseTotaleTrasformata_New = Dose_Tot_New / 1000000
                                            Case enum_UnitaMisura.Quintali
                                                Dose_Trasformata_New = Dose * 100
                                                DoseTotaleTrasformata_New = Dose_Tot_New * 100
                                            Case enum_UnitaMisura.Tonnellate
                                                Dose_Trasformata_New = Dose * 1000
                                                DoseTotaleTrasformata_New = Dose_Tot_New * 1000
                                            Case enum_UnitaMisura.Millilitri
                                                Dose_Trasformata_New = Dose / 1000
                                                DoseTotaleTrasformata_New = Dose_Tot_New / 1000
                                            Case enum_UnitaMisura.CentimetriCubi
                                                Dose_Trasformata_New = Dose / 100
                                                DoseTotaleTrasformata_New = Dose_Tot_New / 100
                                            Case Else
                                                Dose_Trasformata_New = Dose
                                                DoseTotaleTrasformata_New = Dose_Tot_New
                                        End Select

                                        If Fr_Cod <> 0 Then
                                            If Not HashTotNew.ContainsKey(Fr_Cod) Then
                                                HashTotNew.Add(Fr_Cod, DoseTotaleTrasformata_New)
                                            End If
                                        ElseIf Mat_Cod <> 0 Then
                                            If Not HashTotNew.ContainsKey(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto) Then
                                                HashTotNew.Add(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto, DoseTotaleTrasformata_New)
                                            End If
                                        End If

                                        Dose_New = 0
                                        Dose_New_Hl = 0

                                        Select Case Lav_Cod

                                            Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                                 LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                                 LAVCOD_RACCOLTA
                                                'qta è quella totale distribuita
                                                Dose_New = Dose_Tot_New
                                                'modifico la qta del dettaglio (dose_ha)
                                                Dim objDet As New Movimenti_Dettagli_W
                                                objDet.Modifica_Quantita(dettaglio.Piva, dettaglio.Id_Agenda,
                                                                         dettaglio.Id_Mov, dettaglio.Id_Mov_Det,
                                                                         0, 0, 0, 0,
                                                                         Dose_New, 0, 0,
                                                                         0, 0, 0, "", "",
                                                                         objParametri_Server)

                                            Case LAVCOD_IRRIGAZIONE,
                                                 LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA

                                            Case Else

                                                Dose_New = Dose

                                                If AcquaTot <> 0 Then
                                                    Dose_New_Hl = Dose_New * Sup_Tot_New / AcquaTot
                                                End If

                                                'modifico la qta_extra e la qta_extra_tot del dettaglio (dose_hl, qta tot)
                                                Dim objDet As New Movimenti_Dettagli_W
                                                objDet.Modifica_Quantita(dettaglio.Piva, dettaglio.Id_Agenda,
                                                                         dettaglio.Id_Mov, dettaglio.Id_Mov_Det,
                                                                         0, 0, 0, 0,
                                                                         Dose_New, Dose_New_Hl, Dose_Tot_New,
                                                                         0, 0, 0, "", "",
                                                                         objParametri_Server)

                                        End Select


                                        Dim objDest As New Mov_Destinazioni_W

                                        'MOVIMENTI_DESTINAZIONI
                                        If Not IsNothing(dettaglio.Movimenti_Destinazioni) Then
                                            For Each destinazione In dettaglio.Movimenti_Destinazioni
                                                For Each impianto In lista_Impianti
                                                    Dim _piva As String = impianto.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                                                    Dim _sa_cod As Integer = impianto.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                                                    Dim _appezza As Integer = impianto.esercizio.impiantoPK.appezzamentoPK.codice
                                                    Dim _id_reg As Integer = impianto.esercizio.impiantoPK.codice

                                                    If _piva = destinazione.Piva And _sa_cod = destinazione.Sa_Cod And _appezza = destinazione.Appezza And _id_reg = destinazione.Id_Destinazione Then

                                                        Dim QuotaDistribuzioneNew As Decimal = 0
                                                        If Sup_Tot_New <> 0 Then
                                                            QuotaDistribuzioneNew = impianto.esercizio.superficie / Sup_Tot_New
                                                        End If

                                                        Select Case Lav_Cod
                                                            Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA

                                                                objDest.Modifica_SupTrattata(destinazione.Piva,
                                                                                             destinazione.Sa_Cod,
                                                                                             destinazione.Id_Agenda,
                                                                                             destinazione.Id_Mov,
                                                                                             destinazione.Id_Mov_Det,
                                                                                             destinazione.Appezza,
                                                                                             destinazione.Id_Destinazione,
                                                                                             impianto.esercizio.superficie,
                                                                                             QuotaDistribuzioneNew,
                                                                                             "", objParametri_Server)
                                                            Case Else

                                                                Select Case Lav_Cod
                                                                    Case LAVCOD_DISTRIBUZIONE_INSETTI
                                                                        Qta_Dest_New = Math.Round(Dose * impianto.esercizio.superficie, 0)
                                                                    Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                                                         LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                                                         LAVCOD_RACCOLTA
                                                                        'dose salvata sempre qta totale
                                                                        Qta_Dest_New = Math.Round((Dose_New / Sup_Tot_New) * impianto.esercizio.superficie, 0)
                                                                        Dose_Tot_New = Math.Round(Dose_New, 0)

                                                                    Case LAVCOD_IRRIGAZIONE
                                                                        Select Case Dett_Cod
                                                                            Case enum_UnitaMisura.Millimetri
                                                                                Dose = Dose * 10
                                                                            Case enum_UnitaMisura.METRI3__HA
                                                                                Dose = Dose * 1
                                                                            Case Else
                                                                                Dose = 0
                                                                        End Select
                                                                        Qta_Dest_New = Dose * impianto.esercizio.superficie

                                                                    Case Else
                                                                        Qta_Dest_New = Dose_Trasformata_New * impianto.esercizio.superficie
                                                                End Select

                                                                'modifico qta_destinazione e sup_trattata
                                                                objDest.Modifica_Quantita_e_SupTrattata(destinazione.Piva,
                                                                                                        destinazione.Sa_Cod,
                                                                                                        destinazione.Id_Agenda,
                                                                                                        destinazione.Id_Mov,
                                                                                                        destinazione.Id_Mov_Det,
                                                                                                        destinazione.Appezza,
                                                                                                        destinazione.Id_Destinazione,
                                                                                                        Qta_Dest_New,
                                                                                                        impianto.esercizio.superficie,
                                                                                                        QuotaDistribuzioneNew,
                                                                                                        "", objParametri_Server)
                                                        End Select
                                                        Exit For
                                                    End If
                                                Next
                                            Next
                                        End If
                                    Next
                                End If
                        End Select
                    Next


                    'MOVIMENTO SCARICO
                    'modifico la qta del dettaglio e la qta destinazione (magazzino)

                    For Each movimento In Agenda.Movimenti

                        Select Case movimento.Cau_Mov
                            Case CAU_SCARICO, CAU_CARICO

                                If Not IsNothing(movimento.Movimenti_Dettagli) Then

                                    For Each dettaglio In movimento.Movimenti_Dettagli

                                        Fr_Cod = dettaglio.Pro_Cod
                                        Mat_Cod = dettaglio.Mat_Cod
                                        Lotto = dettaglio.Lotto
                                        Udm_Cod = dettaglio.Extra_Int
                                        Udm_Cod_Trasformato = dettaglio.Udm_Cod
                                        Cod_Progetto = dettaglio.Cod_Progetto

                                        Dose = dettaglio.Qta

                                        DoseTotaleTrasformata_New = 0

                                        If Fr_Cod <> 0 Then
                                            If HashTotNew.ContainsKey(Fr_Cod) Then
                                                DoseTotaleTrasformata_New = HashTotNew(Fr_Cod)
                                            End If
                                        Else
                                            If HashTotNew.ContainsKey(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto) Then
                                                DoseTotaleTrasformata_New = HashTotNew(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto)
                                            End If
                                        End If

                                        If DoseTotaleTrasformata_New <> 0 Then

                                            'modifico la qta del dettaglio 
                                            Dim objDet As New Movimenti_Dettagli_W
                                            objDet.Modifica_Quantita(dettaglio.Piva,
                                                                     dettaglio.Id_Agenda,
                                                                     dettaglio.Id_Mov,
                                                                     dettaglio.Id_Mov_Det,
                                                                     0, 0, 0, 0,
                                                                     DoseTotaleTrasformata_New,
                                                                     0, 0,
                                                                     0, 0, 0,
                                                                     "", "", objParametri_Server)


                                            Dim objDest As New Mov_Destinazioni_W

                                            'MOVIMENTI_DESTINAZIONI
                                            If Not IsNothing(dettaglio.Movimenti_Destinazioni) Then

                                                For Each destinazione In dettaglio.Movimenti_Destinazioni
                                                    'modifico qta_destinazione 
                                                    objDest.Modifica_Quantita(destinazione.Piva,
                                                                              destinazione.Sa_Cod,
                                                                              destinazione.Id_Agenda,
                                                                              destinazione.Id_Mov,
                                                                              destinazione.Id_Mov_Det,
                                                                              destinazione.Appezza,
                                                                              destinazione.Id_Destinazione,
                                                                              DoseTotaleTrasformata_New,
                                                                              "", objParametri_Server)
                                                Next
                                            End If
                                        End If
                                    Next
                                End If
                        End Select
                    Next
                End If
            End If

            Dim objAgendaW As New Agenda_W
            Dim Flag_UpdateFlagOK As Boolean

            Flag_UpdateFlagOK = objAgendaW.Agenda_Sblocca2(Piva, Id_Agenda, objParametri_Server.UtenteUsername, Date.Now, "", objParametri_Server)

        Catch ex As Exception

            AgronicaCoreDataProvider.Utility.VerificaAnnullaTransazione(objParametri_Server, False)

            errMess.Add("- " & String.Format(ModificaNonRiuscitaPerOperazioneX, currentAttivitaDes))
            Return False

        Finally
            AgronicaCoreDataProvider.Utility.VerificaChiudiTransazione(objParametri_Server, flagTransazione)
            AgronicaCoreDataProvider.Utility.VerificaChiudiConnessione(objParametri_Server, flagConnessione)
        End Try

        Return True

    End Function

#Region "Utility"
    Private Function check_SelezionatoAlmenoUnElemento(nrElementi As Integer,
                                                       tipo_modifica As enum_ModificaMultiplaOperazioni,
                                                       ByRef messaggio As String) As String

        messaggio = ""

        If nrElementi = 0 Then
            Select Case tipo_modifica
                Case enum_ModificaMultiplaOperazioni.AggiungiMacchina
                    messaggio = SelezionareAlmenoUnaMacchina
                Case enum_ModificaMultiplaOperazioni.AggiungiContatto
                    messaggio = SelezionareAlmenoUnContatto
                Case Else
                    messaggio = SelezionareAlmenoUnOperazione
            End Select
        End If

        Return messaggio
    End Function

    Private Sub generaMessaggioRisultato(nOpModificate As Integer,
                                         nTotOperazioni As Integer,
                                         errMess As List(Of String),
                                         magazziniGiaAssociati As List(Of String),
                                         operazioniNoMagazzino As List(Of String),
                                         ByRef lista_Errori As List(Of ErroreGias))

        Dim mess As String = String.Format(ModificateNOperazioni, nOpModificate)
        If nOpModificate = 1 Then
            mess = ModificataUnaOperazione
        End If

        lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Warning, "", mess, ""))

        If errMess.Count > 0 OrElse magazziniGiaAssociati.Count > 0 OrElse operazioniNoMagazzino.Count > 0 Then
            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Warning, "", String.Format(NonModificateNOperazioni, nTotOperazioni - nOpModificate), ""))
        End If

        If errMess.Count > 0 Then
            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Warning, "", ListaErrori & ":", ""))
            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Warning, "", String.Join(", " & NEWLINE, errMess), ""))
        End If

        If magazziniGiaAssociati.Count > 0 Then
            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Warning, "", EsisteMagazzinoAssociatoSeguentiOperazioni & ":", ""))
            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Warning, "", "- " & String.Join(NEWLINE & "- ", magazziniGiaAssociati), ""))
        End If

        If operazioniNoMagazzino.Count > 0 Then

            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Warning, "", TipologieInterventoNonModificateNonGestisconoScaricoProdotti & ":", ""))
            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Warning, "", "- " & String.Join(NEWLINE & "- ", operazioniNoMagazzino), ""))
        End If

    End Sub

    Private Function scriviDettagli(Piva As String,
                                    Sa_Cod As Integer,
                                    Id_Agenda As Integer,
                                    Id_Mov As Integer,
                                    Id_Mov_Det As Integer,
                                    TipoRisorsa As Integer,
                                    RisorsaCod As Integer,
                                    objParametri_Server As AgronicaCoreParametri
                                    ) As Boolean

        Dim objDettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
        Dim BDummy As Boolean

        Try
            BDummy = objDettagli.Scrivi(Piva, Sa_Cod, Id_Agenda, Id_Mov, Id_Mov_Det, TipoRisorsa,
                                        0, RisorsaCod, "", 0, 0, 0,
                                        0, 0, 0, 0, 0,
                                        0, 0, 0, "", 0, Date.Now,
                                        0, 1900, 0, 0, 0, 0,
                                        1, 3, "", 0, 0, 0,
                                        0, 0, 0, 0, 0,
                                        0, 0, "", 0, 0,
                                        0, "", "", "",
                                        AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
        Catch ex As Exception
            BDummy = False
        End Try

        Return BDummy

    End Function

    Private Function scriviMovimento(Piva As String,
                                     Sa_Cod As Integer,
                                     Id_Agenda As Integer,
                                     Id_Mov As Integer,
                                     Mov_Desc As String,
                                     CAU_MOV As String,
                                     Data As Date,
                                     objParametri_Server As AgronicaCoreParametri
                                     ) As Boolean

        Dim objMovimentoW As New AgronicaCoreContabDAL.Movimenti_W
        Dim BDummy As Boolean = True

        Try
            objMovimentoW.Scrivi(Piva, Sa_Cod, Id_Agenda, Id_Mov,
                                 0, CAU_MOV,
                                 Mov_Desc,
                                 Data, AGRODATAFINE, AGRODATAINIZIO,
                                 0, 0, 0, 0, 0,
                                 0, 0, 0, "", "", 0,
                                 Data, 0, 0, "", 0, Date.Now,
                                 "", "", "", 0, 0, 0,
                                 0, "", 0, 0,
                                 AGRODATAINIZIO, 0, 0,
                                 Data, AGRODATAFINE,
                                 0, objParametri_Server)
        Catch ex As Exception
            BDummy = False
        End Try

        Return BDummy

    End Function

    Private Function generaNuovoIdTabella(NomeTabella As String,
                                          InfoOperazione As InfoOperazione,
                                          objParametri_Server As AgronicaCoreParametri
                                          ) As Integer

        Dim objSequenze As New Agro_Sequenze
        Dim ID As Integer = objSequenze.NuovoId_Tabella(NomeTabella,
                                                        InfoOperazione.BaseCode,
                                                        InfoOperazione.TopCode,
                                                        objParametri_Server)

        Return ID
    End Function
#End Region
End Class
