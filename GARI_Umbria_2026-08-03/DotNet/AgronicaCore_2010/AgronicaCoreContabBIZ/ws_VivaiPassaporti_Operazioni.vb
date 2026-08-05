

Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreVarieBIZ

Public Class ws_VivaiPassaportiOperazioniBiz

    Public Function ConfermaVoceRegistro(piva As String, VivaiPassaporti_Operazione_cod As String, ByVal RecuperaRigheRegistro As Boolean, objParametri_server As AgronicaCoreParametri) As RispostaStandard

        Dim r As New RispostaStandard
        Dim scritturaRegistro As New AgronicaCoreContabDAL.VivaiPassaporti_Operazioni_W
        Dim letturaRegistro As New AgronicaCoreContabDAL.VivaiPassaporti_Operazioni_R


        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                FlagConnessioneLocale,
                FlagTransazioneLocale,
                objParametri_server)


            Dim campConf_W As New AgronicaCoreContabDAL.VivaiPassaporti_Operazioni_W

            r.RispostaOK =
                    scritturaRegistro.ConfermaVoceRegistro(piva, VivaiPassaporti_Operazione_cod, "", objParametri_server)

            Dim messaggiAggiuntivi As String = ""
            If r.RispostaOK Then
                Dim dtVoceRegistro As DataTable =
                    letturaRegistro.Leggi(piva, AGRODATAINIZIO, AGRODATAFINE, " op.ws_VivaiPassaporti_Operazione_cod =  " & VivaiPassaporti_Operazione_cod, "", objParametri_server)


                '_ Non Esiste movimento. _ Esiste riga tabella cod. (MOV-REG-nn)
                If dtVoceRegistro.Rows.Count = 1 AndAlso dtVoceRegistro.Rows(0)("Azione") = "ASS" Then

                    'Sia su Aggiorna che su recupera:

                    'confirm (accetto cancellazione), CANCELLO riga registro, CANCELLO riga transcodifica, come se nulla fosse mai esistito                        
                    scritturaRegistro.Cancella(
                            dtVoceRegistro.Rows(0)("ws_VivaiPassaporti_Operazione_cod"),
                            "",
                            objParametri_server
                        )

                    scritturaRegistro.CancellaLog(
                            dtVoceRegistro.Rows(0)("ws_VivaiPassaporti_Operazione_cod"),
                            dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_piva"),
                            dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_id_agenda"),
                            dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_id_mov"),
                            dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_id_mov_det"),
                            "",
                            objParametri_server
                        )

                End If

                If dtVoceRegistro.Rows.Count = 1 AndAlso dtVoceRegistro.Rows(0)("Azione") = "PEN" Then

                    Dim dtVociCorrelate As DataTable =
                        letturaRegistro.LeggiLog(VivaiPassaporti_Operazione_cod, " l2.azione = 'MOD' ", "", objParametri_server)

                    Dim EFArrayToDelete As New ArrayList
                    For Each rVoce As DataRow In dtVociCorrelate.Rows

                        Dim dtVoceRegistroTest As DataTable =
                            letturaRegistro.Leggi(piva, AGRODATAINIZIO, AGRODATAFINE, " op.ws_VivaiPassaporti_Operazione_cod =  " & rVoce("ws_VivaiPassaporti_Operazione_cod"), "", objParametri_server)

                        If dtVoceRegistroTest.Rows.Count = 1 Then

                            Dim curws_VivaiPassaporti_Operazioni As New ws_VivaiPassaporti_Operazioni
                            curws_VivaiPassaporti_Operazioni.ws_VivaiPassaporti_Operazione_cod = rVoce("ws_VivaiPassaporti_Operazione_cod")
                            EFArrayToDelete.Add(curws_VivaiPassaporti_Operazioni)


                        End If

                    Next


                    Dim MessaggioErrore1 As String = campConf_W.Aggiorna_ws_VivaiPassaporti_Operazioni_W(
                          New ArrayList,
                          New ArrayList,
                          EFArrayToDelete,
                          objParametri_server
                     )

                    'rimuovo solo successivamente i dati
                    For Each rVoce As DataRow In dtVociCorrelate.Rows

                        scritturaRegistro.DocumentiGiasAssegnaFlgInviato(rVoce("ws_VivaiPassaporti_Operazione_cod"), 0, "", objParametri_server)

                        scritturaRegistro.UpdateFlagGestito(
                            rVoce("ws_VivaiPassaporti_Operazione_cod"),
                            rVoce("Doc_in_mov_dettagli_piva"),
                            rVoce("Doc_in_mov_dettagli_id_agenda"),
                            rVoce("Doc_in_mov_dettagli_id_mov"),
                            rVoce("Doc_in_mov_dettagli_id_mov_det"),
                            1,
                            "",
                            objParametri_server
                        )

                    Next

                    Dim rvalDel As Boolean =
                        scritturaRegistro.DocumentiGiasAssegnaAzione(
                            VivaiPassaporti_Operazione_cod, "", "", objParametri_server
                        )

                    scritturaRegistro.DocumentiGiasAssegnaFlgInviato(VivaiPassaporti_Operazione_cod, 0, "", objParametri_server)


                    scritturaRegistro.UpdateFlagGestito(
                        dtVoceRegistro.Rows(0)("ws_VivaiPassaporti_Operazione_cod"),
                        dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_piva"),
                        dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_id_agenda"),
                        dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_id_mov"),
                        dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_id_mov_det"),
                        1,
                        "",
                        objParametri_server
                    )


                    Dim rval1 As Boolean = True
                    If MessaggioErrore1 <> "" Then
                        rval1 = False
                        messaggiAggiuntivi = MessaggioErrore1
                    End If
                    r.RispostaOK = r.RispostaOK And rval1

                    'Else

                    'If dtVoceRegistro.Rows.Count = 1 AndAlso dtVoceRegistro.Rows(0)("Azione") = "" Then
                    '    campConf_W.DocumentiGiasAssegnaAzione(VivaiPassaporti_Operazione_cod, "MODREG", "", objParametri_server )
                    'End If


                End If

                If dtVoceRegistro.Rows.Count = 1 AndAlso dtVoceRegistro.Rows(0)("Azione") = "DEL" AndAlso RecuperaRigheRegistro Then

                    'confirm, SALVO riga, MOV-REG-nn, come se fosse la prima volta
                    scritturaRegistro.DocumentiGiasAssegnaAzione(VivaiPassaporti_Operazione_cod, "", "", objParametri_server)

                    scritturaRegistro.DocumentiGiasAssegnaFlgInviato(VivaiPassaporti_Operazione_cod, 0, "", objParametri_server)

                End If

                'nessuna azioen impostata e mi trovo in Recupera Righe
                If dtVoceRegistro.Rows.Count = 1 AndAlso dtVoceRegistro.Rows(0)("Azione") = "" AndAlso RecuperaRigheRegistro Then

                    scritturaRegistro.DocumentiGiasAssegnaFlgInviato(VivaiPassaporti_Operazione_cod, 0, "", objParametri_server)

                    '' VAnni: 22/5/2020: eventuali elementi di log in stato del vengono eliminati dal log
                    Dim dtVociCorrelate As DataTable =
                        letturaRegistro.LeggiLogElementiConStessaOperazioneAgendaCodDiverso(VivaiPassaporti_Operazione_cod, " l2.azione = 'DEL' ", "", objParametri_server)

                    For each voce In dtVociCorrelate.Rows                        
                            
                        scritturaRegistro.CancellaLog(
                            voce("ws_VivaiPassaporti_Operazione_cod"),
                            voce("Doc_in_mov_dettagli_piva"),
                            voce("Doc_in_mov_dettagli_id_agenda"),
                            voce("Doc_in_mov_dettagli_id_mov"),
                            voce("Doc_in_mov_dettagli_id_mov_det"),
                            "",
                            objParametri_server
                        )

                    Next

                End If

            End If

            r.RispostaStringa = messaggiAggiuntivi

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri_server.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server)

            End If

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            r.RispostaOK = False
            r.Errore = messaggioErrore

        Finally

            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Riporta sulla tabella di registro e sulle tabelle di log i dati dei documenti GIAS
    ''' </summary>
    ''' <param name="RecuperaRigheRegistro"></param>
    ''' <param name="piva"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 
    ''' 
    ''' 
    ''' </remarks>
    Public Function RiportaDocumentiGias(ByVal RecuperaRigheRegistro As Boolean, piva As String, objParametri_Server As AgronicaCoreParametri) As RispostaStandard




        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim rval As New RispostaStandard


        'TODO: Gestire la tranzazione
        Try


            Dim rip As New VivaiPassaporti_Operazioni_W
            Dim lettura As New VivaiPassaporti_Operazioni_R
            Dim sq As New Agro_Sequenze


            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri_Server)

            'prima inizializzazione del log
            Dim logCompilato As Boolean =
                lettura.LogCompilato("", "", objParametri_Server)
            If Not logCompilato Then
                rip.DocumentiGiasRiportoTabellaLog("","", objParametri_Server)
            End If



            'If RecuperaRigheRegistro Then

            '    'mi appoggio sul flag gestita per valorizzare il flag "inviato" in log
            '    rip.DocumentiGiasImpostaFlgInviatoDaGestita(VivaiPassaporti_Operazioni_W.VivaiComportamentoFlginviato.ReimpostaStatoDelSuTabellaLog, "  Gestita = -1 and Azione = 'DEL' ", "", objParametri_Server)
            '    rip.DocumentiGiasImpostaFlgInviatoDaGestita(VivaiPassaporti_Operazioni_W.VivaiComportamentoFlginviato.ReimpostaStatoBlankSuTabellaLog, "  Gestita = -1 and Azione = '' ", "", objParametri_Server)
            '    rip.DocumentiGiasImpostaFlgInviatoDaGestita(VivaiPassaporti_Operazioni_W.VivaiComportamentoFlginviato.ReimpostaStatoMovRegSuTabellaLog, "  Azione = 'MODREG' ", "", objParametri_Server)

            '    'reset sul flag "Gestita" e cancellazione delle righe
            '    rip.UpdateFlagGestito(-1, 0, 0, 0, 0, 2, " or ( Gestita = -1 and Azione = 'DEL')", objParametri_Server)
            '    rip.UpdateFlagGestito(-1, 0, 0, 0, 0, 0, " or Gestita = -1 ", objParametri_Server)
            'Else
            '    rip.UpdateFlagGestito(-1, 0, 0, 0, 0, -1, " or Gestita = 2", objParametri_Server)
            'End If


            '2020-05-22: quello che sto per cancellare [rip.CancellaLog(-1, 0, 0, 0, 0, " or ( Gestita = -1 and Azione = 'DEL')] 
            ' va re-inserito, marco quindi il flag inviato su questi record per il re-inserimento
            Dim dtDaMarcareXcancellati As datatable


            ''versione precedente con cancellazione
            'reset sul flag "Gestita" e cancellazione delle righe
            If RecuperaRigheRegistro Then
                dtDaMarcareXcancellati = lettura.LeggiLog("( Gestita = -1 and Azione = 'DEL' AND Doc_in_mov_dettagli_piva = '" & piva & "') ", "", objParametri_Server)
                rip.CancellaLog(-1, 0, 0, 0, 0, " or ( Gestita = -1 and Azione = 'DEL' and Doc_in_mov_dettagli_piva = '" & piva & "')", objParametri_Server)
                rip.UpdateFlagGestito(-1, 0, 0, 0, 0, 0, " or (Gestita = -1  and Doc_in_mov_dettagli_piva = '" & piva & "')", objParametri_Server)
            End If

            'backup - rimuovere parametro imostaFlaginviato ed usare RecuperaRigheRegistro

            'Riporto dei documenti di trasporto DDT Emessi/Ricevuti (verifica se in formato f&f)
            rip.RiportaDocumentiGias(Not RecuperaRigheRegistro, False, piva, LAVCOD_BOLLA_EMESSA & "," & LAVCOD_BOLLA_RICEVUTA, CAU_REGISTRAZIONI, "", CAU_CARICO, CAU_SCARICO, "Calibro,Provenienza", CostantiPersonalizzate.SEMENTI & ", " & CostantiPersonalizzate.TRASFORMATI_VEGETALI, RecuperaRigheRegistro, "", objParametri_Server)

            'Riporto dei trasferimenti di magazzino
            rip.RiportaDocumentiGias(Not RecuperaRigheRegistro, False, piva, LAVCOD_TRASFERIMENTO, "", "", CAU_CARICO, CAU_SCARICO, "Calibro,Provenienza", CostantiPersonalizzate.SEMENTI & ", " & CostantiPersonalizzate.TRASFORMATI_VEGETALI, RecuperaRigheRegistro, "", objParametri_Server)


            'Riporto dei documenti di conferimento Emessi/Ricevuti
            rip.RiportaDocumentiGias(Not RecuperaRigheRegistro, False, piva, "1054", CAU_REGISTRAZIONE_SECONDARIA, "", CAU_CARICO, CAU_SCARICO, "Calibro,Provenienza", CostantiPersonalizzate.SEMENTI & ", " & CostantiPersonalizzate.TRASFORMATI_VEGETALI, RecuperaRigheRegistro, "", objParametri_Server)

            'Riporto delle lavorazioni
            rip.RiportaDocumentiGias(Not RecuperaRigheRegistro, False, piva, "5000", "10001", "", CAU_CARICO, CAU_SCARICO, "Calibro,Provenienza", CostantiPersonalizzate.SEMENTI & ", " & CostantiPersonalizzate.TRASFORMATI_VEGETALI, RecuperaRigheRegistro, "", objParametri_Server)

            'se sono in modalità di recupero righe registro
            If RecuperaRigheRegistro Then

                'Riporto dei documenti di trasporto DDT Emessi/Ricevuti (verifica se in formato f&f)
                rip.RiportaDocumentiGias(False, True, piva, LAVCOD_BOLLA_EMESSA & "," & LAVCOD_BOLLA_RICEVUTA, CAU_REGISTRAZIONI, "", CAU_CARICO, CAU_SCARICO, "Calibro,Provenienza", CostantiPersonalizzate.SEMENTI & ", " & CostantiPersonalizzate.TRASFORMATI_VEGETALI, RecuperaRigheRegistro, "", objParametri_Server)

                'Riporto dei trasferimenti di magazzino
                rip.RiportaDocumentiGias(false, true, piva, LAVCOD_TRASFERIMENTO, "", "", CAU_CARICO, CAU_SCARICO, "Calibro,Provenienza", CostantiPersonalizzate.SEMENTI & ", " & CostantiPersonalizzate.TRASFORMATI_VEGETALI, RecuperaRigheRegistro, "", objParametri_Server)


                'Riporto dei documenti di conferimento Emessi/Ricevuti
                rip.RiportaDocumentiGias(False, True, piva, "1054", CAU_REGISTRAZIONE_SECONDARIA, "", CAU_CARICO, CAU_SCARICO, "Calibro,Provenienza", CostantiPersonalizzate.SEMENTI & ", " & CostantiPersonalizzate.TRASFORMATI_VEGETALI, RecuperaRigheRegistro, "", objParametri_Server)

                'Riporto delle lavorazioni
                rip.RiportaDocumentiGias(False, True, piva, "5000", "10001", "", CAU_CARICO, CAU_SCARICO, "Calibro,Provenienza", CostantiPersonalizzate.SEMENTI & ", " & CostantiPersonalizzate.TRASFORMATI_VEGETALI, RecuperaRigheRegistro, "", objParametri_Server)


            End If

            rip.DocumentiGiasRiportoTabellaLog(piva, "", objParametri_Server)



            ''backup 2020-05-22
            'rip.DocumentiGiasAssegnaAzioneMOD(False, "azione in ('')", objParametri_Server)

            'If RecuperaRigheRegistro Then
            '    rip.DocumentiGiasAssegnaAzioneMOD(True, "azione in ('', 'MODREG')", objParametri_Server)
            'End If
            ''fine backup 2020-05-22

            If RecuperaRigheRegistro Then
                rip.DocumentiGiasAssegnaAzioneMOD(piva, True, "azione in ('', 'MODREG')", objParametri_Server)

                'quanto precedentemente cancellato viene marcato per essere re-inserito (nuovo 2020-05-22)
                rip.MarcaPerReInserimentoGestitaDel(objParametri_Server, rip, dtDaMarcareXcancellati)

            Else
                rip.DocumentiGiasAssegnaAzioneMOD(piva, False, "azione in ('')", objParametri_Server)
            End If


            rip.DocumentiGiasAssegnaCodiceRiga(piva, "", objParametri_Server)

            'se ho fatto click su "aggiorna" occorre resettare quanto fatto per "recupera":
            '   - rimuovo eventuali righe "provvisorie" dal registro
            '   - re-imposto lo stato precedente sulla tabella di log
            If Not RecuperaRigheRegistro Then
                rip.InserimentoGestitaDel(piva, "", objParametri_Server) '(nuovo 2020-05-22)
                rip.ReImpostaFlagInviatoSuGestitaDel(piva, "", objParametri_Server) '(nuovo 2020-05-22)
                rip.DocumentiGiasRimuoviRigheAggiunteSuRegistroPerRecupera(piva, "", objParametri_Server)
                rip.DocumentiGiasRimuoviRigheAggiunteSuLogPerRecupera(piva, "", objParametri_Server)
                rip.DocumentiGiasImpostaAzioneDaFlgInviato(piva, "", objParametri_Server)
            End If

            'imposto il flag inviato nella tabella log in base al flag "-1" in tabella registro
            If RecuperaRigheRegistro Then
                rip.DocumentiGiasInTabLogUpdateFlgInviatoDaTabRegistro(piva, "", objParametri_Server)
            End If

            If RecuperaRigheRegistro Then
                rip.MarcaDocumentiSenzaPiuAgenda(piva, "", objParametri_Server)
            Else
                rip.MarcaDocumentiSenzaPiuAgenda(piva, " log1.Azione <> 'MODREG' ", objParametri_Server)
            End If

            'sulla tabella principale reset a zero il flag inviato (è di "appoggio" per tabella log"
            rip.DocumentiGiasInTabRegistroResetFlgInviato(piva, 0, "", objParametri_Server)


            rval.RispostaOK = True

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            'fuori transazione ri-allineo i codici
            sq.AllineaUltimoValoreDataTabella("ws_VivaiPassaporti_Operazioni", "ws_VivaiPassaporti_Operazione_cod", objParametri_Server)

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            End If

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            rval.RispostaOK = False
            rval.Errore = messaggioErrore

        Finally

            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try


        Return rval

    End Function


    Private Shared Sub CaricaGriglia_RegistroPassaporti_xJSON_GeneraColonneAnteprima(ByVal dt As DataTable)

        dt.Columns.Add(New DataColumn("CausaleDes", GetType(String)))
        dt.Columns.Add(New DataColumn("CaricoScaricoDes", GetType(String)))

        For Each drDati As DataRow In dt.Rows
            drDati("CausaleDes") = ""
            drDati("CaricoScaricoDes") = ""
        Next

    End Sub

    Private Shared Sub CaricaGriglia_RegistroPassaporti_xJSON_GeneraColonneStampa(ByVal dt As DataTable)

        dt.Columns.Add(New DataColumn("DescrizioneStampa", GetType(String)))
        dt.Columns.Add(New DataColumn("Lingua_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Lingua_Des", GetType(String)))
        dt.Columns.Add(New DataColumn("FF_Stampanti_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("FF_Stampanti_Des", GetType(String)))

        For Each drDati As DataRow In dt.Rows
            drDati("DescrizioneStampa") =
                CType(drDati("DataMovimento"), DateTime).ToShortDateString() & " - " &
                drDati("Lotto_1") & " (" &
                drDati("Veg_Des_Lat") & ", " &
                drDati("Causale") & ", " &
                drDati("CaricoScarico") & ", " &
                drDati("Origine") & ", " &
                drDati("Destinazione") &
                ")"

            drDati("FF_Stampanti_Cod") = 0
            drDati("FF_Stampanti_Des") = ""

            drDati("Lingua_Cod") = 0
            drDati("Lingua_Des") = ""
        Next

    End Sub


    Public Shared Function CaricaGriglia_RegistroPassaporti_xJSON(ByVal dt As DataTable, ByVal TipoGriglia As TipiEnumerativi.enum_PassaportiVivaiTipoGriglia) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome


        c = New ColonneNome("ws_VivaiPassaporti_Operazione_cod", "ws_VivaiPassaporti_Operazione_cod", "number")
        c._hidden = True
        l.Add(c)

        If TipoGriglia = TipiEnumerativi.enum_PassaportiVivaiTipoGriglia.Stampa Then

            CaricaGriglia_RegistroPassaporti_xJSON_GeneraColonneStampa(dt)

            c = New ColonneNome("DescrizioneStampa", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_Griglia_PP_Descrizione, "string")
            l.Add(c)

            'i18n Traduzione ad ora non necessaria
            c = New ColonneNome("Lingua_Cod", "Lingua_cod", "number")
            c._Display = False
            c._Editabile = True
            l.Add(c)

            'i18n Traduzione ad ora non necessaria
            c = New ColonneNome("Lingua_Des", "Lingua_Des", "string")
            c._Display = False
            c._Editabile = True
            l.Add(c)

            c = New ColonneNome("FF_Stampanti_Cod", "FF_Stampanti_Cod", "string")
            c._Display = False
            c._Editabile = True
            c._hidden = True
            l.Add(c)


            c = New ColonneNome("FF_Stampanti_Des", "FF_Stampanti_Des", "string")
            c._Editabile = True
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Qta1", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Numero, "number")
            c._Editabile = True
            c._hidden = True
            l.Add(c)

        Else

            'CaricaGriglia_RegistroPassaporti_xJSON_GeneraColonneAnteprima(dt)

            c = New ColonneNome("GIAS_Stato", "GIAS_Stato", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("CodiceRiga", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_CodiceRiga, "string")
            c._Editabile = False
            l.Add(c)

            c = New ColonneNome("WAnagraficaStati_DES", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Stato, "string")
            c._Editabile = False
            l.Add(c)


            c = New ColonneNome("statoColore", "statoColore", "string")
            c._hidden = True
            l.Add(c)

            'A
            c = New ColonneNome("Veg_Des_Lat", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Nome_Botanico_Specie & "  *", "string")
            c._hidden = True
            c._Display = False
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)

            'B

            c = New ColonneNome("Codice_RUOP", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_Griglia_PP_Codice_RUOP & "  *", "string")
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)


            'C            
            c = New ColonneNome("Lotto_1", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Codice_di_tracciabilità_aziendale & "  *", "string")
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)

            'D            
            c = New ColonneNome("PaeseDiOrigine", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Paese_Di_Origine & "  *", "string")
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)

            c = New ColonneNome("Azione", "Azione", "string")
            c._hidden = True
            l.Add(c)


            c = New ColonneNome("Vivaista_PIVA", "Vivaista_PIVA", "string")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Rif_Distinta_Piva", "Rif_Distinta_Piva", "string")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Rif_Distinta_Progetto_Cod", "Rif_Distinta_Progetto_Cod", "number")
            c._hidden = True
            l.Add(c)


            c = New ColonneNome("Rag_Soc_RUOP", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_Griglia_Rag_Soc_RUOP & "  *", "string")
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)


            c = New ColonneNome("SitoCodice", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_Griglia_Codice_Sito, "string")
            c._Editabile = True
            'c._obbligatorio = true
            l.Add(c)

            c = New ColonneNome("SitoRagioneSociale", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_Griglia_Rag_Soc_Sito, "string")
            c._Editabile = True
            'c._obbligatorio = true
            l.Add(c)



            c = New ColonneNome("TipoZona", "Tipo Zona", "string")
            c._hidden = True
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)

            c = New ColonneNome("TipoZonaDes", "TipoZonaDes", "string")
            c._hidden = True
            c._Display = False
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)

            c = New ColonneNome("DDT_in_Numero", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_DDT_Ricevuto, "string")
            c._Editabile = True
            l.Add(c)

            c = New ColonneNome("DDT_out_Numero", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_DDT_Emesso, "string")
            c._Editabile = True
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Doc_in_mov_dettagli_piva", "Doc_in_mov_dettagli_piva", "string")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Doc_in_mov_dettagli_sa_cod", "Doc_in_mov_dettagli_sa_cod", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Doc_in_mov_dettagli_id_agenda", "Doc_in_mov_dettagli_id_agenda", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Doc_in_mov_dettagli_id_mov", "Doc_in_mov_dettagli_id_mov", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Doc_in_mov_dettagli_id_mov_det", "Doc_in_mov_dettagli_id_mov_det", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Doc_out_mov_dettagli_piva", "Doc_out_mov_dettagli_piva", "string")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Doc_out_mov_dettagli_sa_cod", "Doc_out_mov_dettagli_sa_cod", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Doc_out_mov_dettagli_id_agenda", "Doc_out_mov_dettagli_id_agenda", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Doc_out_mov_dettagli_id_mov", "Doc_out_mov_dettagli_id_mov", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Doc_out_mov_dettagli_id_mov_det", "Doc_out_mov_dettagli_id_mov_det", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("DataMovimento", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Data_Movimento & "  *", "date")
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)


            c = New ColonneNome("Causale", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Causale, "string")
            c._hidden = True
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)

            c = New ColonneNome("CausaleDes", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Causale, "string")
            c._hidden = True
            c._Display = False
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)

            c = New ColonneNome("CaricoScarico", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Carico_Scarico, "string")
            c._hidden = True
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)

            c = New ColonneNome("CaricoScaricoDes", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Carico_Scarico, "string")
            c._hidden = True
            c._Display = False
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)

            c = New ColonneNome("Origine", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Origine, "string")
            c._Editabile = True
            c._obbligatorio = True
            'c._hidden = true
            l.Add(c)

            c = New ColonneNome("Destinazione", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Destinazione & "  *", "string")
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)
            c = New ColonneNome("FornitoreAPO", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Fornitore_APO, "string")
            c._Editabile = True
            c._Display = False
            l.Add(c)

            c = New ColonneNome("Cul_COD", "Cul_COD", "number")
            c._hidden = True
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)

            c = New ColonneNome("Grfri_Cod", "Grfri_Cod", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Grva_Cod", "Grva_Cod", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("elem_cod", "elem_cod", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("mat_cod", "mat_cod", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("cal_cod", "cal_cod", "number")
            c._hidden = True
            l.Add(c)


            c = New ColonneNome("cul_Des", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Varietà, "string")
            c._Editabile = True
            c._Display = False
            l.Add(c)

            c = New ColonneNome("Prodotto_Des", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Prodotto, "string")
            c._Editabile = True
            l.Add(c)

            c = New ColonneNome("Qta1", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Quantità_Pedane_bins & "  *", "number")
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)

            c = New ColonneNome("Qta2", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Quantità_Imballi & "  *", "number")
            c._Editabile = True
            c._obbligatorio = True
            c._Display = False
            l.Add(c)

            c = New ColonneNome("Qta3", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Quantità_Confezioni, "number")
            c._Editabile = True
            l.Add(c)

            c = New ColonneNome("Calibro", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Calibro, "string")
            c._Editabile = True
            l.Add(c)

            c = New ColonneNome("Lotto_2", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Lotto_2, "string")
            c._Editabile = True
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Coltivatore_codRisum", "Coltivatore_codRisum", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Coltivatore_RagioneSociale", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Coltivatore_Ragione_Sociale, "string")
            c._Editabile = True
            l.Add(c)
            c = New ColonneNome("Coltivatore_Indirizzo", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Coltivatore_Indirizzo, "string")
            c._Editabile = True
            l.Add(c)
            c = New ColonneNome("Coltivatore_Cap", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Coltivatore_Cap, "string")
            c._Editabile = True
            c._hidden = True
            l.Add(c)
            c = New ColonneNome("Coltivatore_Citta", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Coltivatore_Città, "string")
            c._Editabile = True
            c._hidden = True
            l.Add(c)
            c = New ColonneNome("Coltivatore_Regione", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Coltivatore_Regione, "string")
            c._Editabile = True
            l.Add(c)

            c = New ColonneNome("DestinazioneFinale_codRisum", "DestinazioneFinale_codRisum", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("DestinazioneFinale_RagioneSociale", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_DestinazioneFinale_Ragione_Sociale, "string")
            c._Editabile = True
            l.Add(c)
            c = New ColonneNome("DestinazioneFinale_Indirizzo", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_DestinazioneFinale_Indirizzo, "string")
            c._Editabile = True
            l.Add(c)
            c = New ColonneNome("DestinazioneFinale_Cap", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_DestinazioneFinale_Cap, "string")
            c._Editabile = True
            c._hidden = True
            l.Add(c)
            c = New ColonneNome("DestinazioneFinale_Citta", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_DestinazioneFinale_Città, "string")
            c._Editabile = True
            c._hidden = True
            l.Add(c)
            c = New ColonneNome("DestinazioneFinale_Regione", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_DestinazioneFinale_Regione, "string")
            c._Editabile = True
            l.Add(c)
            c = New ColonneNome("Preso_in_cosegna_Da", My.Resources.AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz_CaricaGriglia_RegistroPassaporti_xJSON_Preso_in_consegna_da, "string")
            c._Editabile = True
            l.Add(c)
            c = New ColonneNome("Note", Gias.Note, "string")
            c._Editabile = True
            l.Add(c)


        End If
        'tipo Griglia stampa ? 

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipiEnumerativi.TipoFiltroKendo_colonne.Menu) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '

        Return risp


    End Function


End Class

Public Class ws_VivaiPassaporti_Operazioni_W
    Inherits AgronicaCoreDataProvider.LogProvider


#Region "Costruttori"

    Public Sub New()
        Provider = Globalization.CultureInfo.InvariantCulture
        Format = "yyyyMMdd"
        ValiditaInizio = Date.ParseExact("19000101", Format, Provider)
        ValiditaFine = Date.ParseExact("21001231", Format, Provider)
    End Sub

#End Region

    Private _format As String
    Public Shadows Property Format() As String
        Get
            Return _format
        End Get
        Set
            _format = Value
        End Set
    End Property

    Private _provider As Globalization.CultureInfo
    Public Shadows Property Provider() As Globalization.CultureInfo
        Get
            Return _provider
        End Get
        Set
            _provider = Value
        End Set
    End Property


    Private _validitaInizio As Date
    Public Shadows Property ValiditaInizio() As Date
        Get
            Return _validitaInizio
        End Get
        Set
            _validitaInizio = Value
        End Set
    End Property

    Private _validitaFine As Date
    Public Shadows Property ValiditaFine() As Date
        Get
            Return _validitaFine
        End Get
        Set
            _validitaFine = Value
        End Set
    End Property


    Public Function Aggiorna_ws_VivaiPassaporti_Operazioni(
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByVal recuperaRigheRegistro As Boolean,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "ws_VivaiPassaporti_Operazioni_W.Aggiorna_ws_VivaiPassaporti_Operazioni_W()"



        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            Dim operazioniR As New VivaiPassaporti_Operazioni_R

            Dim curws_VivaiPassaporti_Operazioni As New ws_VivaiPassaporti_Operazioni


            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                FlagConnessioneLocale,
                FlagTransazioneLocale,
                objParametri)


            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In righeInseriteArray
                curws_VivaiPassaporti_Operazioni = New ws_VivaiPassaporti_Operazioni
                curws_VivaiPassaporti_Operazioni.ws_VivaiPassaporti_Operazione_cod = 0

                ws_VivaiPassaportiLeggiJson(curws_VivaiPassaporti_Operazioni, obj)

                curws_VivaiPassaporti_Operazioni.GIAS_Stato = TipiEnumerativi.enum_WWorflow_WAnagraficaStati.Registro_Carico_Scarico_Passaporti_Vivaisti_Confermato


                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    curws_VivaiPassaporti_Operazioni.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                Else
                    curws_VivaiPassaporti_Operazioni.Validita_Inizio = ValiditaInizio
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    curws_VivaiPassaporti_Operazioni.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                Else
                    curws_VivaiPassaporti_Operazioni.Validita_Fine = ValiditaFine
                End If
                curws_VivaiPassaporti_Operazioni.Data_Creazione = Date.Now
                curws_VivaiPassaporti_Operazioni.Username_Creazione = objParametri.UsernameOperazione
                curws_VivaiPassaporti_Operazioni.Data_Modifica = Date.Now
                curws_VivaiPassaporti_Operazioni.Username_Modifica = objParametri.UsernameOperazione
                curws_VivaiPassaporti_Operazioni.inviato = 0

                EFArrayToInsert.Add(curws_VivaiPassaporti_Operazioni)
            Next


            For Each obj As JObject In righeModificateArray


                curws_VivaiPassaporti_Operazioni = operazioniR.LeggiEFSingolo(obj("ws_VivaiPassaporti_Operazione_cod"), objParametri)
                If curws_VivaiPassaporti_Operazioni Is Nothing Then
                    MessaggioErrore += "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else

                    ws_VivaiPassaportiLeggiJson(curws_VivaiPassaporti_Operazioni, obj)

                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curws_VivaiPassaporti_Operazioni.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    Else
                        curws_VivaiPassaporti_Operazioni.Validita_Fine = ValiditaFine
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curws_VivaiPassaporti_Operazioni.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    Else
                        curws_VivaiPassaporti_Operazioni.Validita_Fine = ValiditaFine
                    End If
                    curws_VivaiPassaporti_Operazioni.Data_Modifica = Date.Now
                    curws_VivaiPassaporti_Operazioni.Username_Modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(curws_VivaiPassaporti_Operazioni)

                End If
            Next

            Dim listaFlagGestita As New DataTable
            listaFlagGestita.Columns.Add("ws_VivaiPassaporti_Operazione_cod", Type.GetType("System.Int32"))
            listaFlagGestita.Columns.Add("Doc_in_mov_dettagli_piva", Type.GetType("System.String"))
            listaFlagGestita.Columns.Add("Doc_in_mov_dettagli_id_agenda", Type.GetType("System.Int32"))
            listaFlagGestita.Columns.Add("Doc_in_mov_dettagli_id_mov", Type.GetType("System.Int32"))
            listaFlagGestita.Columns.Add("Doc_in_mov_dettagli_id_mov_det", Type.GetType("System.Int32"))
            listaFlagGestita.Columns.Add("azione", Type.GetType("System.String"))


            Dim campConf_W As New AgronicaCoreContabDAL.VivaiPassaporti_Operazioni_W
            Dim letturaRegistro As New AgronicaCoreContabDAL.VivaiPassaporti_Operazioni_R
            For Each obj As JObject In righeCancellateArray

                Dim AccodaPerCancellazione As Boolean = True

                curws_VivaiPassaporti_Operazioni = New ws_VivaiPassaporti_Operazioni
                Dim ws_VivaiPassaporti_Operazione_cod_Elim As Integer =
                    obj("ws_VivaiPassaporti_Operazione_cod")
                curws_VivaiPassaporti_Operazioni.ws_VivaiPassaporti_Operazione_cod = ws_VivaiPassaporti_Operazione_cod_Elim
                curws_VivaiPassaporti_Operazioni.Vivaista_PIVA = obj("Vivaista_PIVA")


                Dim dtVoceRegistro As DataTable =
                    letturaRegistro.Leggi(obj("Vivaista_PIVA"), AGRODATAINIZIO, AGRODATAFINE, " op.ws_VivaiPassaporti_Operazione_cod =  " & ws_VivaiPassaporti_Operazione_cod_Elim, "", objParametri)

                If dtVoceRegistro.Rows(0)("Azione") = "ASS" Then


                    AccodaPerCancellazione = False

                    'sia sul pulsante aggiorna che sul pulsante recupera:
                    'tengo la mia riga e scrivo nella transcodofica che ho rifiutato la proposta
                    campConf_W.DocumentiGiasAssegnaAzione(ws_VivaiPassaporti_Operazione_cod_Elim, "MODREG", "", objParametri)

                    'reset del flag inviato
                    campConf_W.DocumentiGiasAssegnaFlgInviato(ws_VivaiPassaporti_Operazione_cod_Elim, 0, "", objParametri)

                    campConf_W.UpdateFlagGestito(
                        dtVoceRegistro.Rows(0)("ws_VivaiPassaporti_Operazione_cod"),
                        dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_piva"),
                        dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_id_agenda"),
                        dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_id_mov"),
                        dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_id_mov_det"),
                        1,
                        "",
                        objParametri
                    )

                End If

                If dtVoceRegistro.Rows(0)("Azione") = "PEN" Then

                    Dim dtVociCorrelate As DataTable =
                       letturaRegistro.LeggiLog(ws_VivaiPassaporti_Operazione_cod_Elim, " l2.azione = 'MOD' ", "", objParametri)


                    For Each voceDelAssociazione In dtVociCorrelate.Rows
                        Dim riga As DataRow = listaFlagGestita.NewRow()
                        riga("ws_VivaiPassaporti_Operazione_cod") = voceDelAssociazione("ws_VivaiPassaporti_Operazione_cod")
                        riga("Doc_in_mov_dettagli_piva") = voceDelAssociazione("Doc_in_mov_dettagli_piva")
                        riga("Doc_in_mov_dettagli_id_agenda") = voceDelAssociazione("Doc_in_mov_dettagli_id_agenda")
                        riga("Doc_in_mov_dettagli_id_mov") = voceDelAssociazione("Doc_in_mov_dettagli_id_mov")
                        riga("Doc_in_mov_dettagli_id_mov_det") = voceDelAssociazione("Doc_in_mov_dettagli_id_mov_det")
                        riga("Azione") = dtVoceRegistro.Rows(0)("Azione")

                        listaFlagGestita.Rows.Add(riga)
                    Next


                    campConf_W.DocumentiGiasAssegnaFlgInviato(ws_VivaiPassaporti_Operazione_cod_Elim, 0, "", objParametri)

                    campConf_W.UpdateFlagGestito(
                        dtVoceRegistro.Rows(0)("ws_VivaiPassaporti_Operazione_cod"),
                        dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_piva"),
                        dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_id_agenda"),
                        dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_id_mov"),
                        dtVoceRegistro.Rows(0)("Doc_in_mov_dettagli_id_mov_det"),
                        -1,
                        "",
                        objParametri
                    )

                End If

                If dtVoceRegistro.Rows(0)("Azione") = "DEL" Then
                    AccodaPerCancellazione = False
                End If

                If AccodaPerCancellazione Then
                    EFArrayToDelete.Add(curws_VivaiPassaporti_Operazioni)
                End If
            Next




            If String.IsNullOrEmpty(MessaggioErrore) Then

                MessaggioErrore = campConf_W.Aggiorna_ws_VivaiPassaporti_Operazioni_W(
                      EFArrayToInsert,
                      EFArrayToUpdate,
                      EFArrayToDelete,
                      objParametri
                 )

            End If



            For Each rigaEliminata As ws_VivaiPassaporti_Operazioni In EFArrayToDelete
                If letturaRegistro Is Nothing Then
                    letturaRegistro = New VivaiPassaporti_Operazioni_R
                End If


                For Each rVoce As DataRow In listaFlagGestita.Rows

                    campConf_W.DocumentiGiasAssegnaFlgInviato(rVoce("ws_VivaiPassaporti_Operazione_cod"), 0, "", objParametri)

                    campConf_W.UpdateFlagGestito(
                        rVoce("ws_VivaiPassaporti_Operazione_cod"),
                        rVoce("Doc_in_mov_dettagli_piva"),
                        rVoce("Doc_in_mov_dettagli_id_agenda"),
                        rVoce("Doc_in_mov_dettagli_id_mov"),
                        rVoce("Doc_in_mov_dettagli_id_mov_det"),
                        -1,
                        "",
                        objParametri
                    )

                Next



            Next

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)


        Catch ex As Exception
            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)

            End If


            MessaggioErrore = "[" & NomeRoutine & "] : " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function


    Private Sub ws_VivaiPassaportiLeggiJson(curws_VivaiPassaporti_Operazioni As ws_VivaiPassaporti_Operazioni, obj As JObject)

        curws_VivaiPassaporti_Operazioni.GIAS_Stato = If(Not String.IsNullOrEmpty(obj("GIAS_Stato")), CInt(obj("GIAS_Stato")), 0)
        curws_VivaiPassaporti_Operazioni.Vivaista_PIVA = If(Not String.IsNullOrEmpty(obj("Vivaista_PIVA")), CStr(obj("Vivaista_PIVA")), "")
        curws_VivaiPassaporti_Operazioni.Rif_Distinta_Piva = If(Not String.IsNullOrEmpty(obj("Rif_Distinta_Piva")), CStr(obj("Rif_Distinta_Piva")), "")
        curws_VivaiPassaporti_Operazioni.Rif_Distinta_Progetto_Cod = If(Not String.IsNullOrEmpty(obj("Rif_Distinta_Progetto_Cod")), CInt(obj("Rif_Distinta_Progetto_Cod")), 0)
        curws_VivaiPassaporti_Operazioni.Rag_Soc_RUOP = If(Not String.IsNullOrEmpty(obj("Rag_Soc_RUOP")), CStr(obj("Rag_Soc_RUOP")), "")
        curws_VivaiPassaporti_Operazioni.Codice_RUOP = If(Not String.IsNullOrEmpty(obj("Codice_RUOP")), CStr(obj("Codice_RUOP")), "")
        curws_VivaiPassaporti_Operazioni.TipoZona = If(Not String.IsNullOrEmpty(obj("TipoZona")), CStr(obj("TipoZona")), "")
        curws_VivaiPassaporti_Operazioni.DDT_in_Numero = If(Not String.IsNullOrEmpty(obj("DDT_in_Numero")), CStr(obj("DDT_in_Numero")), "")
        curws_VivaiPassaporti_Operazioni.DDT_out_Numero = If(Not String.IsNullOrEmpty(obj("DDT_out_Numero")), CStr(obj("DDT_out_Numero")), "")
        curws_VivaiPassaporti_Operazioni.Doc_in_mov_dettagli_piva = If(Not String.IsNullOrEmpty(obj("Doc_in_mov_dettagli_piva")), CStr(obj("Doc_in_mov_dettagli_piva")), "")
        curws_VivaiPassaporti_Operazioni.Doc_in_mov_dettagli_sa_cod = If(Not String.IsNullOrEmpty(obj("Doc_in_mov_dettagli_sa_cod")), CInt(obj("Doc_in_mov_dettagli_sa_cod")), 0)
        curws_VivaiPassaporti_Operazioni.Doc_in_mov_dettagli_id_agenda = If(Not String.IsNullOrEmpty(obj("Doc_in_mov_dettagli_id_agenda")), CInt(obj("Doc_in_mov_dettagli_id_agenda")), 0)
        curws_VivaiPassaporti_Operazioni.Doc_in_mov_dettagli_id_mov = If(Not String.IsNullOrEmpty(obj("Doc_in_mov_dettagli_id_mov")), CInt(obj("Doc_in_mov_dettagli_id_mov")), 0)
        curws_VivaiPassaporti_Operazioni.Doc_in_mov_dettagli_id_mov_det = If(Not String.IsNullOrEmpty(obj("Doc_in_mov_dettagli_id_mov_det")), CInt(obj("Doc_in_mov_dettagli_id_mov_det")), 0)
        curws_VivaiPassaporti_Operazioni.Doc_out_mov_dettagli_piva = If(Not String.IsNullOrEmpty(obj("Doc_out_mov_dettagli_piva")), CStr(obj("Doc_out_mov_dettagli_piva")), "")
        curws_VivaiPassaporti_Operazioni.Doc_out_mov_dettagli_sa_cod = If(Not String.IsNullOrEmpty(obj("Doc_out_mov_dettagli_sa_cod")), CInt(obj("Doc_out_mov_dettagli_sa_cod")), 0)
        curws_VivaiPassaporti_Operazioni.Doc_out_mov_dettagli_id_agenda = If(Not String.IsNullOrEmpty(obj("Doc_out_mov_dettagli_id_agenda")), CInt(obj("Doc_out_mov_dettagli_id_agenda")), 0)
        curws_VivaiPassaporti_Operazioni.Doc_out_mov_dettagli_id_mov = If(Not String.IsNullOrEmpty(obj("Doc_out_mov_dettagli_id_mov")), CInt(obj("Doc_out_mov_dettagli_id_mov")), 0)
        curws_VivaiPassaporti_Operazioni.Doc_out_mov_dettagli_id_mov_det = If(Not String.IsNullOrEmpty(obj("Doc_out_mov_dettagli_id_mov_det")), CInt(obj("Doc_out_mov_dettagli_id_mov_det")), 0)
        curws_VivaiPassaporti_Operazioni.DataMovimento = If(Not String.IsNullOrEmpty(obj("DataMovimento")), CDate(obj("DataMovimento")), AGRODATAINIZIO)
        curws_VivaiPassaporti_Operazioni.PaeseDiOrigine = If(Not String.IsNullOrEmpty(obj("PaeseDiOrigine")), CStr(obj("PaeseDiOrigine")), "")
        curws_VivaiPassaporti_Operazioni.Causale = If(Not String.IsNullOrEmpty(obj("Causale")), CStr(obj("Causale")), "")
        curws_VivaiPassaporti_Operazioni.CaricoScarico = If(Not String.IsNullOrEmpty(obj("CaricoScarico")), CStr(obj("CaricoScarico")), "")
        curws_VivaiPassaporti_Operazioni.Origine = If(Not String.IsNullOrEmpty(obj("Origine")), CStr(obj("Origine")), "")
        curws_VivaiPassaporti_Operazioni.Destinazione = If(Not String.IsNullOrEmpty(obj("Destinazione")), CStr(obj("Destinazione")), "")
        curws_VivaiPassaporti_Operazioni.FornitoreAPO = If(Not String.IsNullOrEmpty(obj("FornitoreAPO")), CStr(obj("FornitoreAPO")), "")
        curws_VivaiPassaporti_Operazioni.Cul_COD = If(Not String.IsNullOrEmpty(obj("Cul_COD")), CInt(obj("Cul_COD")), 0)
        curws_VivaiPassaporti_Operazioni.Grfri_Cod = If(Not String.IsNullOrEmpty(obj("Grfri_Cod")), CInt(obj("Grfri_Cod")), 0)
        curws_VivaiPassaporti_Operazioni.Grva_Cod = If(Not String.IsNullOrEmpty(obj("Grva_Cod")), CInt(obj("Grva_Cod")), 0)
        curws_VivaiPassaporti_Operazioni.elem_cod = If(Not String.IsNullOrEmpty(obj("elem_cod")), CInt(obj("elem_cod")), 0)
        curws_VivaiPassaporti_Operazioni.mat_cod = If(Not String.IsNullOrEmpty(obj("mat_cod")), CInt(obj("mat_cod")), 0)
        curws_VivaiPassaporti_Operazioni.cal_cod = If(Not String.IsNullOrEmpty(obj("cal_cod")), CInt(obj("cal_cod")), 0)
        curws_VivaiPassaporti_Operazioni.Veg_Des_Lat = If(Not String.IsNullOrEmpty(obj("Veg_Des_Lat")), CStr(obj("Veg_Des_Lat")), "")
        curws_VivaiPassaporti_Operazioni.cul_Des = If(Not String.IsNullOrEmpty(obj("cul_Des")), CStr(obj("cul_Des")), "")
        curws_VivaiPassaporti_Operazioni.Prodotto_Des = If(Not String.IsNullOrEmpty(obj("Prodotto_Des")), CStr(obj("Prodotto_Des")), "")
        curws_VivaiPassaporti_Operazioni.Qta1 = If(Not String.IsNullOrEmpty(obj("Qta1")), CInt(obj("Qta1")), 0)
        curws_VivaiPassaporti_Operazioni.Qta2 = If(Not String.IsNullOrEmpty(obj("Qta2")), CInt(obj("Qta2")), 0)
        curws_VivaiPassaporti_Operazioni.Qta3 = If(Not String.IsNullOrEmpty(obj("Qta3")), CInt(obj("Qta3")), 0)
        curws_VivaiPassaporti_Operazioni.Calibro = If(Not String.IsNullOrEmpty(obj("Calibro")), CStr(obj("Calibro")), "")
        curws_VivaiPassaporti_Operazioni.Lotto_1 = If(Not String.IsNullOrEmpty(obj("Lotto_1")), CStr(obj("Lotto_1")), "")
        curws_VivaiPassaporti_Operazioni.Lotto_2 = If(Not String.IsNullOrEmpty(obj("Lotto_2")), CStr(obj("Lotto_2")), "")
        curws_VivaiPassaporti_Operazioni.Coltivatore_codRisum = If(Not String.IsNullOrEmpty(obj("Coltivatore_codRisum")), CInt(obj("Coltivatore_codRisum")), 0)
        curws_VivaiPassaporti_Operazioni.Coltivatore_RagioneSociale = If(Not String.IsNullOrEmpty(obj("Coltivatore_RagioneSociale")), CStr(obj("Coltivatore_RagioneSociale")), "")
        curws_VivaiPassaporti_Operazioni.Coltivatore_Indirizzo = If(Not String.IsNullOrEmpty(obj("Coltivatore_Indirizzo")), CStr(obj("Coltivatore_Indirizzo")), "")
        curws_VivaiPassaporti_Operazioni.Coltivatore_Cap = If(Not String.IsNullOrEmpty(obj("Coltivatore_Cap")), CStr(obj("Coltivatore_Cap")), "")
        curws_VivaiPassaporti_Operazioni.Coltivatore_Citta = If(Not String.IsNullOrEmpty(obj("Coltivatore_Citta")), CStr(obj("Coltivatore_Citta")), "")
        curws_VivaiPassaporti_Operazioni.Coltivatore_Regione = If(Not String.IsNullOrEmpty(obj("Coltivatore_Regione")), CStr(obj("Coltivatore_Regione")), "")
        curws_VivaiPassaporti_Operazioni.DestinazioneFinale_codRisum = If(Not String.IsNullOrEmpty(obj("DestinazioneFinale_codRisum")), CInt(obj("DestinazioneFinale_codRisum")), 0)
        curws_VivaiPassaporti_Operazioni.DestinazioneFinale_RagioneSociale = If(Not String.IsNullOrEmpty(obj("DestinazioneFinale_RagioneSociale")), CStr(obj("DestinazioneFinale_RagioneSociale")), "")
        curws_VivaiPassaporti_Operazioni.DestinazioneFinale_Indirizzo = If(Not String.IsNullOrEmpty(obj("DestinazioneFinale_Indirizzo")), CStr(obj("DestinazioneFinale_Indirizzo")), "")
        curws_VivaiPassaporti_Operazioni.DestinazioneFinale_Cap = If(Not String.IsNullOrEmpty(obj("DestinazioneFinale_Cap")), CStr(obj("DestinazioneFinale_Cap")), "")
        curws_VivaiPassaporti_Operazioni.DestinazioneFinale_Citta = If(Not String.IsNullOrEmpty(obj("DestinazioneFinale_Citta")), CStr(obj("DestinazioneFinale_Citta")), "")
        curws_VivaiPassaporti_Operazioni.DestinazioneFinale_Regione = If(Not String.IsNullOrEmpty(obj("DestinazioneFinale_Regione")), CStr(obj("DestinazioneFinale_Regione")), "")
        curws_VivaiPassaporti_Operazioni.Preso_in_cosegna_Da = If(Not String.IsNullOrEmpty(obj("Preso_in_cosegna_Da")), CStr(obj("Preso_in_cosegna_Da")), "")
        curws_VivaiPassaporti_Operazioni.Note = If(Not String.IsNullOrEmpty(obj("Note")), CStr(obj("Note")), "")
        curws_VivaiPassaporti_Operazioni.SitoCodice = If(Not String.IsNullOrEmpty(obj("SitoCodice")), CStr(obj("SitoCodice")), "")
        curws_VivaiPassaporti_Operazioni.SitoRagioneSociale = If(Not String.IsNullOrEmpty(obj("SitoRagioneSociale")), CStr(obj("SitoRagioneSociale")), "")
    End Sub
End Class