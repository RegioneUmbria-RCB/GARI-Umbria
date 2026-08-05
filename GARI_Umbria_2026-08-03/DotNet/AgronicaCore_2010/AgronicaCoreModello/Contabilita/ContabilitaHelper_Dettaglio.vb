Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreModelsSTD.exceptions

Public Class ContabilitaHelper_Dettaglio

    Private _objParametriServer As AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreParametri
    Private _lavCodScaricoRaccolta As Integer() = {LAVCOD_BOLLA_EMESSA, LAVCOD_SCARICO}
    Private _raccolteImpLotto As String


    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtenti As AgronicaCoreParametri)

        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti
    End Sub

    'TODO: cosa mi serve dei padri per salvare correttamente dettaglio?
    'Movimento di intestazione (4000):
    '- Id_Mov (devo aggiornare peso, num colli, provvigioni in euro ...)
    '- Data operazione
    '- Username_Modifica

    'Movimento di carico/scarico (7300-7350)
    '- Se non l'avevo già creato, lo devo creare (cosa mi serve?)
    '- Cau_Mov (devo sapere se scarico/carico)
    '- Lav_Cod (simile punto sopra?)

    Public Function AggiornaSingolaRigaDocumento(ByVal objContabDettaglio As Contabilita_Riga,
                                                 ByVal lavCod As Integer,
                                                 ByVal cauMov As String,
                                                 ByVal idMovT As Integer,
                                                 ByVal idMovCS As Integer,
                                                 ByVal dataOp As Date,
                                                 ByVal usernameOperatore As String,
                                                 ByVal messaggioDettagliato As Boolean,
                                                 Optional ByVal objContabTestata As Contabilita_Testata = Nothing,
                                                 Optional ByVal Id_Agenda_GHG As Integer = 0
                                                 ) As Contabilita_Output


        Const nomeRoutine = "ContabilitaHelper.AggiornaSingolaRigaDocumento()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim xRisp As Boolean = False
        Dim msgError As String = ""
        Dim flagOggettoCorretto As Boolean
        Dim conteggiTotali As Contabilita_Totali_Testata = Nothing
        Dim FiltroAggiuntivoRiferimenti As String = ""

        Dim agendaHelper As New Agenda_Operazione_Helper
        Dim movHelper As New Agenda_Movimenti_Helper
        Dim movDetHelper As New Agenda_Movimenti_Dettagli_Helper
        Dim movDestHelper As New Agenda_Movimenti_Destinazioni_Helper
        Dim movDetTecExW As New Mov_Dett_Tecnico_Ex_W

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyRigaContabilita(objContabDettaglio, enum_TipoOperazioneDB.Modifica, _objParametriServer)

            If flagOggettoCorretto Then

                'Prima di procedere con la modifica devo verificare se nel frattempo il record dell'agenda è cambiato
                Dim dataUltimaModifica As DateTime
                Dim usernameUltimaModifica As String = ""

                dataUltimaModifica = UtilityHelper.UltimaModificaMovDettaglio(objContabDettaglio.Piva,
                                                                              objContabDettaglio.SaCod,
                                                                              objContabDettaglio.IdAgenda,
                                                                              objContabDettaglio.IdMov,
                                                                              objContabDettaglio.IdMovDet,
                                                                              usernameUltimaModifica,
                                                                              _objParametriServer)

                If Not IsNothing(objContabDettaglio.DataOraUltimaLettura) AndAlso dataUltimaModifica > objContabDettaglio.DataOraUltimaLettura Then
                    msgError &= UtilityHelper.GetMessaggioDataModifica(dataUltimaModifica, usernameUltimaModifica, _objParametriUtenti)
                    xRisp = False
                Else

                    'TODO: se ho più di una riga devo spezzare!!!
                    Dim objOutputSpacca As Contabilita_Output = Nothing
                    Dim flagSpacca As Boolean = False
                    If objContabDettaglio.Confezionamenti IsNot Nothing AndAlso objContabDettaglio.Confezionamenti.Count > 1 Then

                        flagSpacca = True
                        objOutputSpacca = SpaccaRigheConfezionamentiDiversi(objContabDettaglio,
                                                                            lavCod, cauMov,
                                                                            idMovT,
                                                                            objContabDettaglio.IdMov,
                                                                            dataOp, usernameOperatore,
                                                                            messaggioDettagliato, msgError, objContabTestata)

                        If objOutputSpacca IsNot Nothing AndAlso objOutputSpacca.Risultato = True Then
                            xRisp = True
                            conteggiTotali = objOutputSpacca.ConteggiTotali
                        Else
                            xRisp = False
                            msgError &= objOutputSpacca.MsgError
                        End If

                    Else

                        'Ho singola riga confezionamenti o nessuna ==> proseguo con la modifica (cancello e riscrivo dettaglio) vera e propria

                        'Per evitare problemi nel cambio categoria e similari, ogni volta elimino la riga dettaglio precedente

                        'Leggo cosa c'è scritto ora sul db
                        Dim movDetPrecedentiList = movDetHelper.Leggi(objContabDettaglio.Piva, 0,
                                                                      objContabDettaglio.IdAgenda,
                                                                      objContabDettaglio.IdMov,
                                                                      objContabDettaglio.IdMovDet,
                                                                      _objParametriServer)

                        If movDetPrecedentiList Is Nothing OrElse movDetPrecedentiList.Count <> 1 Then
                            Throw New Exception("ERRORE: sono in modifica ma non ho trovato il movimento precedente")
                        End If


                        Dim movDetPrecedente As Movimento_Dettaglio = movDetPrecedentiList(0)

                        'TODO: IMPERATIVO!!! devo conservare le chiavi!!!

                        'chiavi da tenere: Id_Mov_Det, Id_Destinazione (non serve, perché se non è cambiato è uguale, altrimenti è giusto che cambi),
                        ' Id_Reg_Dettaglio di Movimento_Tecnico (NO, potrei non averlo), Id_Reg_Dettaglio di Movimento_Tecnico_Extra (NO, potrei non averlo),
                        ' Cal_Cod (?!? in realtà dovrei prendere quello di interfaccia, se ho cambiato prodotto è giusto che cambi)
                        '
                        'Se era il carico, quindi era l'unico punto dove era usato quel cal_cod, vengono eliminate le materie prime collegate in fase di cancellazione?!?

                        'TODO: richiamare cancellazione con verifica (in modalità silenziosa), perché deve cancellare anche eventuale raccolta collegata
                        'Dim xRispCanc As Boolean = movDetHelper.Cancella(movDetPrecedente.Piva, 0,
                        '                                                 movDetPrecedente.Id_Agenda,
                        '                                                 movDetPrecedente.Id_Mov,
                        '                                                 movDetPrecedente.Id_Mov_Det,
                        '                                                 _objParametriServer)

                        'If xRispCanc = False Then
                        '    Throw New Exception("Impossibile sovrascrivere il dettaglio! Errore nella cancellazione")
                        'End If

                        Dim agendeRaccolteAutomatiche As New List(Of Operazione_Agenda)()
                        Dim agendaGHGRegistrazioneCreataInAutomatico As Operazione_Agenda

                        If objContabDettaglio.Tipo_Associazione = 0 AndAlso
                               UtilityHelper.IsAccettazione(lavCod) = True AndAlso
                               UtilityHelper.IsTrasformatoFF(objContabDettaglio.ModuloGias, objContabDettaglio.ElemCod) AndAlso
                               objContabDettaglio.PivaConferente <> "" Then

                            'Sono in modifica di una riga di conferimento: controllo se era stata creata una raccolta automatica, ed in caso ne leggo i dati
                            'prima della cancellazione perché ho bisogno di mantenerne alcuni alla "ri-scrittura"
                            'TODO Verificare il caso limite per il quale la raccolta era associata ad un produttore e questo è stato cambiato
                            Dim elencoRifRaccolte = objContabDettaglio.ListRifMovDettaglio.Where(Function(detRif) detRif.Lav_Cod_Rif = LAVCOD_RACCOLTA).ToList()

                            For Each rifRaccolta In elencoRifRaccolte
                                agendeRaccolteAutomatiche.Add(agendaHelper.Leggi(rifRaccolta.Piva_Rif, 0, rifRaccolta.Id_Agenda_Rif, 0, _objParametriServer))
                            Next

                        End If

                        Dim msgCancellazione As String = ""
                        Dim modProtetta As Integer = 0
                        Dim bPreserva_GHG As Boolean = True

                        'Controllo Eliminazione Parametri GHG
                        If objContabDettaglio.ElemCod = RIGA_DESCRIZIONE_LIBERA Or objContabDettaglio.ElemCod = ALTRI_BENI Or IsNothing(objContabDettaglio.GHG_Registrazioni) Then

                            bPreserva_GHG = False

                            '===================================================================================================
                            'Elimino in riscrittura l'eventuale record del riferimento
                            '---------------------------------------------------------------------------------------------------
                            If Not IsNothing(objContabDettaglio.ListRifMovDettaglio) Then

                                For i = 0 To objContabDettaglio.ListRifMovDettaglio.Count - 1

                                    If objContabDettaglio.ListRifMovDettaglio(i).Lav_Cod_Rif = LAVCOD_GHG Then
                                        objContabDettaglio.ListRifMovDettaglio.Remove(objContabDettaglio.ListRifMovDettaglio(i))
                                        Exit For
                                    End If

                                Next

                            End If
                            '===================================================================================================

                        End If

                        Dim r As AgronicaCoreVarieBIZ.RispostaStandard = UtilityHelper.CancellaDocumento(_objParametriServer,
                                                                                                        movDetPrecedente.Piva,
                                                                                                        movDetPrecedente.Id_Agenda,
                                                                                                        movDetPrecedente.Id_Mov,
                                                                                                        movDetPrecedente.Id_Mov_Det,
                                                                                                        lavCod,
                                                                                                        enum_TipoOperazioneDB.Cancellazione,
                                                                                                        IgnoraAvvisoWarning:=True,
                                                                                                        Bypass_Delete_Exceptions:=True,
                                                                                                        ModuloGiasLicenziato:=objContabDettaglio.ModuloGias,
                                                                                                        flagAggiornaConteggi:=False,
                                                                                                        messaggio:=msgCancellazione,
                                                                                                        Modalita_Protetta:=modProtetta,
                                                                                                        Preserva_Costi:=True,
                                                                                                        objParametriUtenti:=_objParametriUtenti,
                                                                                                        Preserva_GHG:=bPreserva_GHG)

                        If r Is Nothing OrElse r.RispostaOK = False Then
                            Dim m As String = r.Errore
                            Throw New Exception("Impossibile sovrascrivere il dettaglio! Errore nella cancellazione" & If(m <> "", ": " & m, ""))
                        End If

                        Dim objDettaglio As Movimento_Dettaglio = GeneraRigaContabilita(objContabDettaglio,
                                                                                    lavCod,
                                                                                    cauMov,
                                                                                    idMovT,
                                                                                    idMovCS,
                                                                                    dataOp,
                                                                                    usernameOperatore,
                                                                                    movDetPrecedente)

                        If objDettaglio Is Nothing Then
                            Throw New Exception("Non è stato possibile creare l'oggetto Riga Dettaglio")
                        End If

                        Dim objDetHelper As New Agenda_Movimenti_Dettagli_Helper
                        Dim idMovDet = objDetHelper.Scrivi(objDettaglio, _objParametriServer)

                        If idMovDet > 0 Then

                            If UtilityHelper.IsTrasformatoFF(objContabDettaglio.ModuloGias, objContabDettaglio.ElemCod) Then

                                UtilityHelper.SistemaImballaggiDocumento(objContabDettaglio.Piva, objContabDettaglio.IdAgenda, idMovCS,
                                                                        enum_TipoOperazioneDB.Modifica, _objParametriServer,
                                                                        objContabDettaglio.ModuloGias, objDettaglio)

                            End If

                            'TODO: aggiorno colli testata + peso lordo testata (+ eventualmente provvigione, castelletto, ecc...)
                            conteggiTotali = UtilityHelper.AggiornaConteggiTotali(objContabDettaglio.Piva, lavCod,
                                                                                objContabDettaglio.IdAgenda,
                                                                                _objParametriServer,
                                                                                idMovT)

                            xRisp = True

                            'Scrittura Raccolte collegate a Conferimento: se create automaticamente hanno Tipo_Associazione impostato a zero,
                            'altrimenti se pre-esistenti è impostato ad uno
                            If objContabDettaglio.Tipo_Associazione = 0 AndAlso
                                UtilityHelper.IsAccettazione(lavCod) = True AndAlso
                                UtilityHelper.IsTrasformatoFF(objContabDettaglio.ModuloGias, objContabDettaglio.ElemCod) AndAlso
                                objContabDettaglio.PivaConferente <> "" Then

                                'Modifica del 11/05/2022: Tolgo la possibilità di scrivere la raccolta con impianti indefiniti
                                If (Not IsNothing(objContabDettaglio.RigheImpianti) AndAlso objContabDettaglio.RigheImpianti.Count > 0) Then

                                    Dim outpRaccolteAutomatiche = ScriviRaccolteCollegate(objContabDettaglio, objContabTestata, lavCod, cauMov, dataOp, enum_TipoOperazioneDB.Modifica, agendeRaccolteAutomatiche)

                                    If outpRaccolteAutomatiche.Risultato = True Then
                                        xRisp = True
                                    Else
                                        xRisp = False
                                        msgError &= outpRaccolteAutomatiche.MsgError
                                    End If

                                End If

                            End If

                            'Scrittura Tabella GHG_Registrazioni
                            If objContabDettaglio.ElemCod <> RIGA_DESCRIZIONE_LIBERA And objContabDettaglio.ElemCod <> ALTRI_BENI Then

                                If (Not IsNothing(objContabDettaglio.GHG_Registrazioni) AndAlso objContabDettaglio.GHG_Registrazioni.Count > 0) Then

                                    Id_Agenda_GHG = ScriviGHGRegistazioneCollegata(objContabDettaglio.GHG_Registrazioni, objContabDettaglio, dataOp, objContabTestata.DescrizioneAgenda, lavCod, Id_Agenda_GHG)

                                    If Id_Agenda_GHG > 0 Then
                                        xRisp = True
                                    Else
                                        xRisp = False
                                        msgError &= "Si è verificato un errore durante il salvataggio dei parametri GHG"
                                    End If

                                End If

                            End If

                            If objContabDettaglio.Tipo_Associazione = 1 AndAlso
                            UtilityHelper.IsAccettazione(lavCod) = True AndAlso
                            UtilityHelper.IsTrasformatoFF(objContabDettaglio.ModuloGias, objContabDettaglio.ElemCod) AndAlso
                            objContabDettaglio.PivaConferente <> "" Then

                                Dim outputAggiornaRaccolte = AggiornaRaccoltaAziendaCampagna(objContabDettaglio, objContabTestata, enum_TipoOperazioneDB.Modifica)

                                If outputAggiornaRaccolte.Risultato = True Then
                                    xRisp = True
                                Else
                                    xRisp = False
                                    msgError &= outputAggiornaRaccolte.MsgError
                                End If

                            End If

                            'TODO: aggiornare se sono cambiati Des_Lib e Data_Movimento
                            Dim objAgronicaLogAgendaW As New AgronicaCoreContabDAL.AgronicaLogAgenda_W
                            objAgronicaLogAgendaW.Scrivi(dataOp,
                                                        enum_TipoOperazioneDB.Modifica,
                                                        "Des_Lib da inserire in AggiornaSingolaRigaDocumento",
                                                        objContabDettaglio.IdAgenda,
                                                        objContabDettaglio.Piva,
                                                        0,
                                                        lavCod,
                                                        CInt(enum_Id_Servizio.GiasOnline),
                                                        _objParametriServer)

                        Else
                            Throw New Exception("ERRORE: non sono riuscito a riscrivere il movimento dettaglio")
                        End If

                    End If

                End If

            End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return New Contabilita_Output With {
            .Risultato = xRisp,
            .MsgError = msgError,
            .Id_Mov_Carico = objContabDettaglio.IdMov,
            .Id_Mov_Scarico = objContabDettaglio.IdMov,
            .Id_Mov_Det = objContabDettaglio.IdMovDet,
            .ConteggiTotali = conteggiTotali
        }

    End Function

    Private Class CentriRaccolte
        Public Sa_Cod As Integer
        Public Qta As Decimal
        Public Veg_Des As String

        Public Sub New()
            Sa_Cod = 0
            Qta = 0
            Veg_Des = ""
        End Sub
    End Class

    Public Class ImpiantiRaccolte
        Public Piva As String
        Public Sa_Cod As Integer
        Public Appezza As Integer
        Public Id_Reg As Integer
        Public Qta As Decimal
        Public Sup_Trattata As Decimal

        Public Sub New()
            Piva = ""
            Sa_Cod = 0
            Appezza = 0
            Id_Reg = 0
            Qta = 0
            Sup_Trattata = 0
        End Sub
    End Class

    Private Function ScriviRaccolteCollegate(ByRef objContabDettaglio As Contabilita_Riga,
                                        ByRef objContabTestata As Contabilita_Testata,
                                        ByVal lavCod As Integer,
                                        ByVal cauMov As String,
                                        ByVal dataOp As Date,
                                        ByVal operazioneSvoltaSulConferimento As enum_TipoOperazioneDB,
                                        Optional ByVal agendeRaccoltePrecedenti As List(Of Operazione_Agenda) = Nothing) As Contabilita_Output

        Const nomeRoutine = "ContabilitaHelper.ScriviRaccolteCollegate()"
        Dim ObjAgenda As Operazione_Agenda
        Dim dr_search() As DataRow
        Dim bDuplicato As Boolean
        Dim Id_Agenda_Raccolta As Integer
        Dim ArrayCentri As New List(Of CentriRaccolte)
        Dim ArrayImpianti As New List(Of ImpiantiRaccolte)
        Dim Sa_Cod As Integer = -1
        Dim objImpreseImpostazioni As New Imprese_Impostazioni_R
        Dim setupMagazzinoSuRaccolta As Boolean = True 'Attiva di default
        Dim objContabOutput As New Contabilita_Output() With {
            .Risultato = True
        }

        Try
            'Leggo l'impostazione su database "Raccolta con carichi di magazzino" utilizzando la piva dell'azienda agricola e non il sa_cod
            Dim strSetupMagazzinoSuRaccolta = objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser(objContabDettaglio.PivaConferente, New List(Of Integer)({0}), enum_Impostazioni_Utenti.Raccolta_Con_Carico_Magazzino, "", _objParametriUtenti, _objParametriServer)

            If strSetupMagazzinoSuRaccolta = "0" Then
                setupMagazzinoSuRaccolta = False
            End If

            Select Case objContabDettaglio.ChkImpiantiIndefiniti

                Case True

                    'Impianti Indefiniti -> Questo caso non si può più verificare dalla versione 10-06-2022

                    'Lettura del Primo Centro Aziendale valido per la specie vegetale
                    Dim leggiImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                    Dim dtImpianti As New DataTable

                    dtImpianti = leggiImpianti.Leggi_DescrizioniImpianti(objContabDettaglio.PivaConferente, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", _objParametriServer)

                    If dtImpianti.Rows.Count > 0 Then

                        Sa_Cod = dtImpianti(0)("Sa_Cod")

                        'Tentativo: Cerco il Centro con Specie Vegetale Compatibile
                        dr_search = dtImpianti.Select("Veg_Cod = " & objContabDettaglio.VegCod)

                        If dr_search.Count > 0 Then

                            'Correggo il Sa_Cod
                            Sa_Cod = dr_search(0)("Sa_Cod")

                        End If

                        'Veg_Des = "" Non conosco la specie vegetale (amen)
                        ArrayCentri.Add(New CentriRaccolte With {
                                           .Sa_Cod = Sa_Cod,
                                           .Qta = CDec(objContabDettaglio.KgNetti),
                                           .Veg_Des = ""
                                        })

                        ArrayImpianti.Add(New ImpiantiRaccolte With {
                                             .Piva = objContabDettaglio.PivaConferente,
                                             .Sa_Cod = Sa_Cod,
                                             .Appezza = 0,
                                             .Id_Reg = 0,
                                             .Qta = CDec(objContabDettaglio.KgNetti)
                                          })

                    End If

                Case False

                    'Ho delle righe di impianto ==> Devo scrivere 1 o + raccolte

                    'Determinazione dei Centri Aziendali in Struttura di Appoggio
                    For Each objImpianto In objContabDettaglio.RigheImpianti

                        bDuplicato = False

                        For i = 0 To ArrayCentri.Count - 1

                            If ArrayCentri(i).Sa_Cod = objImpianto.Sa_Cod Then

                                bDuplicato = True

                                'Aggiornamento QtaxCentro
                                ArrayCentri(i).Qta += objImpianto.Qta

                                Exit For

                            End If

                        Next

                        If Not bDuplicato Then

                            ArrayCentri.Add(New CentriRaccolte With {
                                               .Sa_Cod = objImpianto.Sa_Cod,
                                               .Qta = objImpianto.Qta,
                                               .Veg_Des = objImpianto.Veg_Des
                                            })
                        End If

                        'Inserisco Impianto in Struttura Appoggio
                        ArrayImpianti.Add(New ImpiantiRaccolte With {
                                             .Piva = objImpianto.Piva,
                                             .Sa_Cod = objImpianto.Sa_Cod,
                                             .Appezza = objImpianto.Appezza,
                                             .Id_Reg = objImpianto.Id_Reg,
                                             .Qta = objImpianto.Qta,
                                             .Sup_Trattata = objImpianto.Sup_Imp
                                          })

                    Next

            End Select

            Dim objRiferimentoHelper As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
            Dim Dummy As Boolean
            Dim dettagliCaricoRaccolte As New List(Of Movimento_Dettaglio)()

            'Inserisco 1 raccolta per centro aziendale valido
            For i = 0 To ArrayCentri.Count - 1

                ObjAgenda = CreaOggettoRaccolteCollegate(objContabDettaglio,
                                                         objContabDettaglio.PivaConferente,
                                                         ArrayCentri(i).Sa_Cod,
                                                         ArrayCentri(i).Qta,
                                                         ArrayImpianti,
                                                         dataOp,
                                                         ArrayCentri(i).Veg_Des,
                                                         objContabDettaglio.NoteRaccolta,
                                                         setupMagazzinoSuRaccolta,
                                                         agendeRaccoltePrecedenti)


                'Scrittura Raccolta
                If Not IsNothing(ObjAgenda) Then
                    Dim objAgendaHelper As New Agenda_Operazione_Helper
                    Id_Agenda_Raccolta = objAgendaHelper.Scrivi(ObjAgenda, _objParametriServer, flagUsaOraReale:=True)

                    '------------------------------------------------
                    '----- RIFERIMENTO OPERAZIONI DI AGENDA
                    '------------------------------------------------
                    Dim objMovCaricoRaccolta As New Movimento
                    Dim Id_Mov_Carico_Raccolta As Integer = -1
                    Dim Id_Mov_Det_Carico_Raccolta As Integer = -1

                    If setupMagazzinoSuRaccolta = True Then
                        objMovCaricoRaccolta = ObjAgenda.Movimenti.Where(Function(mov) mov.Cau_Mov = CAU_CARICO).First()

                        Id_Mov_Carico_Raccolta = objMovCaricoRaccolta.Id_Mov
                        Id_Mov_Det_Carico_Raccolta = objMovCaricoRaccolta.Movimenti_Dettagli.First().Id_Mov_Det
                    End If

                    Dim objRiferimento = New Movimento_Dettaglio_Riferimento With {
                        .Piva = objContabDettaglio.Piva,
                        .Sa_Cod = objContabDettaglio.DestinazioneCarico.SaCod,
                        .Id_Agenda = objContabDettaglio.IdAgenda,
                        .Id_Mov = objContabDettaglio.IdMov,
                        .Id_Mov_Det = objContabDettaglio.IdMovDet,
                        .Lav_Cod = lavCod,
                        .Cau_Mov = cauMov,
                        .Piva_Rif = CStr(objContabDettaglio.PivaConferente),
                        .Sa_Cod_Rif = ArrayCentri(i).Sa_Cod,
                        .Id_Agenda_Rif = Id_Agenda_Raccolta,
                        .Id_Mov_Rif = Id_Mov_Carico_Raccolta,
                        .Id_Mov_Det_Rif = Id_Mov_Det_Carico_Raccolta,
                        .Lav_Cod_Rif = LAVCOD_RACCOLTA,
                        .Cau_Mov_Rif = If(setupMagazzinoSuRaccolta, CAU_CARICO, CAU_RILIEVO_RACCOLTA),
                        .Tipo_Associazione = 0
                    }

                    Dummy = objRiferimentoHelper.Scrivi(objRiferimento, _objParametriServer)

                    If setupMagazzinoSuRaccolta = True Then
                        dettagliCaricoRaccolte.AddRange(objMovCaricoRaccolta.Movimenti_Dettagli)
                    End If

                End If

            Next i

            Dim bollaEmessaIdAgenda As Integer

            If operazioneSvoltaSulConferimento = enum_TipoOperazioneDB.Modifica AndAlso objContabDettaglio.ListRifMovDettaglio.Count > 0 Then

                If agendeRaccoltePrecedenti IsNot Nothing AndAlso agendeRaccoltePrecedenti.Count > 0 Then

                    For Each r In agendeRaccoltePrecedenti
                        'La cancellazione della riga di conferimento al fine di riscriverla cancella anche gli eventuali riferimenti delle raccolte automatiche
                        'ove queste apparivano nella parte principale del record, pertanto se c'erano li riscrivo
                        Dim detRifCancellati = r.Agenda_Riferimenti.Where(Function(detRif) detRif.Lav_Cod = LAVCOD_RACCOLTA)

                        For Each detRif As Movimento_Dettaglio_Riferimento In detRifCancellati
                            Dummy = objRiferimentoHelper.Scrivi(detRif, _objParametriServer)
                        Next
                    Next

                End If

                'Recupero l'eventuale id_agenda del ddt emesso che scarica il prodotto che ho caricato con le raccolte sull'azienda agricola
                Dim rifCancellatoBollaEmessa = objContabDettaglio.ListRifMovDettaglio.Where(Function(detRif) _lavCodScaricoRaccolta.Contains(detRif.Lav_Cod_Rif) AndAlso detRif.Tipo_Associazione = 2).ToList()
                bollaEmessaIdAgenda = If(rifCancellatoBollaEmessa.Count > 0, rifCancellatoBollaEmessa.First().Id_Agenda_Rif, 0)

            End If

            If objContabTestata Is Nothing Then
                objContabOutput = ScriviScaricoRaccolteAziendaAgricola(operazioneSvoltaSulConferimento, objContabDettaglio.Piva, objContabDettaglio.IdAgenda, objContabDettaglio.IdMov, objContabDettaglio.IdMovDet, bollaEmessaIdAgenda, dettagliCaricoRaccolte)
            Else
                objContabOutput = ScriviScaricoRaccolteAziendaAgricola(operazioneSvoltaSulConferimento, objContabTestata.Piva, objContabTestata.IdAgenda, objContabDettaglio.IdMov, objContabDettaglio.IdMovDet, bollaEmessaIdAgenda, dettagliCaricoRaccolte)
            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return objContabOutput

    End Function

    Public Function CreaOggettoRaccolteCollegate(ByVal ObjDettaglio As Contabilita_Riga,
                                                 ByVal Piva As String,
                                                 ByVal Sa_Cod As Integer,
                                                 ByVal Qta_Ripartita As Decimal,
                                                 ByVal ArrayImpianti As List(Of ImpiantiRaccolte),
                                                 ByVal Data_Movimento As Date,
                                                 ByVal Veg_Des As String,
                                                 ByVal Note As String,
                                                 ByVal setupMagazzinoSuRaccolta As Boolean,
                                                 ByVal agendeRaccoltePrecedenti As List(Of Operazione_Agenda)
                                                 ) As Operazione_Agenda

        Const nomeRoutine = "ContabilitaHelper.CreaOggettoRaccolteCollegate()"
        Dim objAgenda As Operazione_Agenda = Nothing
        Dim CodRisumFornitore As Integer = 0
        Dim objSequenze As New Agro_Sequenze
        Dim dalFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R()
        Dim dtMagazzino As New DataTable()
        Dim magazzinoCarico As Integer = 0
        Dim handleImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read()

        Try

            Dim lottoRaccolta = RaccolteOttieniLotto(ObjDettaglio, Data_Movimento)

            Dim idAgendaPrecedente As Integer

            If agendeRaccoltePrecedenti IsNot Nothing AndAlso agendeRaccoltePrecedenti.Count > 0 Then
                idAgendaPrecedente = agendeRaccoltePrecedenti.First(Function(a) a.Sa_Cod = Sa_Cod).Id_Agenda
            End If

            '------------------------------------------------
            '----- AGENDA
            '------------------------------------------------

            objAgenda = New Operazione_Agenda With {
                .Tipo_Operazione = enum_TipoOperazioneDB.Scrittura,
                .Id_Agenda = idAgendaPrecedente,
                .Data = Data_Movimento,
                .Piva = Piva,
                .Sa_Cod = Sa_Cod,
                .Lav_Cod = LAVCOD_RACCOLTA,
                .Linea_Cod = 0,
                .Preparazione_Cod = 0,
                .Id_Trasformazione = 0,
                .Des_Lib = "Rilievo Produzione " & Veg_Des & " Lotto " & lottoRaccolta
            }

            '------------------------------------------------
            '----- MOVIMENTI
            '------------------------------------------------

            'MOVIMENTO DI RACCOLTA
            Dim objMovimentoT = New Movimento With {
                .Piva = objAgenda.Piva,
                .Id_Agenda = 0, 'Non serve specificarlo qua perché la funzione di scrittura sovrascrive questo valore con quello dell'oggetto Operazione_Agenda
                .Id_Mov = 0,
                .Sa_Cod = Sa_Cod,
                .Cod_Risum = 0,
                .Cod_IndirizzoRisUm = 0,
                .Data = Data_Movimento,
                .Lav_Cod = objAgenda.Lav_Cod,
                .Cau_Mov = CAU_RILIEVO_RACCOLTA,
                .Mov_Desc = Note,
                .Colli = 0,
                .Tipo_Peso = enum_TipoPeso.Peso_Netto,
                .Peso = Qta_Ripartita,
                .Ora = Data_Movimento,
                .Extra_Date = #12/30/1899#,
                .Data_Registrazione = AGRODATAINIZIO,
                .Extra_Int = If(setupMagazzinoSuRaccolta = True, enum_RACCOLTA_TIPO.Leggera_Con_Dettagli_Magazzino, enum_RACCOLTA_TIPO.Leggera)
            }

            objMovimentoT.Id_Mov = objSequenze.NuovoId_Tabella("Movimenti",
                                        objMovimentoT.BaseCode,
                                        objMovimentoT.TopCode,
                                        _objParametriServer)


            '------------------------------------------------
            '----- MOVIMENTI DETTAGLI
            '------------------------------------------------

            Dim objMovDettagliT = New Movimento_Dettaglio With {
                .Piva = objAgenda.Piva,
                .Sa_Cod = Sa_Cod,
                .Id_Agenda = 0,  'Non serve specificarlo qua perché la funzione di scrittura sovrascrive questo valore con quello dell'oggetto Movimento
                .Id_Mov = 0,  'Non serve specificarlo qua perché la funzione di scrittura sovrascrive questo valore con quello dell'oggetto Movimento
                .Id_Mov_Det = 0,
                .Lav_Cod = objAgenda.Lav_Cod,
                .Elem_Cod = CInt(ObjDettaglio.ElemCod),
                .Pro_Cod = 0,
                .Mat_Cod = CInt(ObjDettaglio.MatCod),
                .Mov_Det_Des = "",
                .Qta = Qta_Ripartita,
                .Qta_Extra_Totale = Qta_Ripartita,
                .Cal_Cod = 0, 'Questo dato non viene più valorizzato nelle raccolte
                .Extra_Int = 0, 'Questo dato non viene più valorizzato nelle raccolte
                .Udm_Cod = ObjDettaglio.DB_Udm_Cod,
                .Jolly_Int = 0,
                .Contabilizzato = NONCONTABILE,
                .Pendente = enum_Pendenza.MovGiustificato,
                .Lotto = lottoRaccolta,
                .Tara = CDec(ObjDettaglio.Tara),
                .Udm_Cod_Extra = 0,
                .Qta_Extra = 0,
                .Ordine_Det = 0,
                .Validita_Inizio = Data_Movimento,
                .Validita_Fine = AGRODATAFINE
            }

            objMovDettagliT.Id_Mov_Det = objSequenze.NuovoId_Tabella("Movimenti_Dettagli",
                                                               objMovDettagliT.BaseCode,
                                                               objMovDettagliT.TopCode,
                                                               _objParametriServer)

            '------------------------------------------------
            '----- MOVIMENTI DESTINAZIONI
            '------------------------------------------------
            'Aggancio la destinazione al dettaglio
            objMovDettagliT.Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

            Dim superficieTrattataTotale As Decimal = (
                From imp In ArrayImpianti
                Where imp.Sa_Cod = Sa_Cod
                Select imp.Sup_Trattata).Sum()

            'For indice = 0 To UBound(ArrayImpianti, 2) - 1
            For indice = 0 To ArrayImpianti.Count - 1

                'Controllo Sa_Cod
                If Sa_Cod = ArrayImpianti(indice).Sa_Cod Then

                    Dim objMovDestinazioniT = New Movimento_Destinazione With {
                        .Piva = objAgenda.Piva,
                        .Sa_Cod = Sa_Cod,
                        .Id_Agenda = 0,
                        .Id_Mov = objMovimentoT.Id_Mov,
                        .Id_Mov_Det = objMovDettagliT.Id_Mov_Det,
                        .Tipo = 0,
                        .Appezza = CInt(ArrayImpianti(indice).Appezza),
                        .Id_Destinazione = CInt(ArrayImpianti(indice).Id_Reg),
                        .Qta = CDec(ArrayImpianti(indice).Qta),
                        .Qta2 = ArrayImpianti(indice).Sup_Trattata,
                        .QuotaDistribuzione = ArrayImpianti(indice).Sup_Trattata / superficieTrattataTotale
                    }

                    objMovDettagliT.Movimenti_Destinazioni.Add(objMovDestinazioniT)
                    objMovDestinazioniT = Nothing

                End If

            Next



            'Aggancio i movimenti dettagli al movimento
            objMovimentoT.Movimenti_Dettagli = New List(Of Movimento_Dettaglio) From {objMovDettagliT}
            objMovDettagliT = Nothing

            'Aggancio movimento testata ad agenda
            objAgenda.Movimenti = New List(Of Movimento) From {objMovimentoT}
            objMovimentoT = Nothing


            If setupMagazzinoSuRaccolta = True Then

                'MOVIMENTO DI CARICO
                Dim objMovimentoC = CreaOggettoMovimentoCaricoRaccolta(objAgenda.Piva, Sa_Cod, idAgendaPrecedente, Data_Movimento, objAgenda.Lav_Cod, 0, Qta_Ripartita, ObjDettaglio)

                'Aggancio movimento carico ad agenda
                objAgenda.Movimenti.Add(objMovimentoC)
            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return objAgenda

    End Function

    Private Function CreaOggettoMovimentoCaricoRaccolta(ByVal piva As String, ByVal saCod As Integer, ByVal idAgenda As Integer, ByVal dataMov As Date, ByVal lavCod As Integer, ByVal matCodRaccolta As Integer, ByVal qta As Decimal, ByRef dettaglioConf As Contabilita_Riga) As Movimento

        Const nomeRoutine = "ContabilitaHelper_Dettaglio.CreaOggettoMovimentoCaricoRaccolta()"

        Dim objMovimentoC As Movimento
        Dim objMovDettagliC As Movimento_Dettaglio

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim helperOpAgenda As New Agenda_Operazione_Helper()
        Dim dalFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R()
        Dim objSequenze As New Agro_Sequenze()
        Dim handleImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read()

        Dim saCodCarico As Integer = 0
        Dim idMagazzinoDest As Integer = 0
        Dim dtMagazzino As New DataTable()

        Try
            'Devo creare un nuovo movimento con cau_mov 7300 sulla operazione di raccolta - Nel data-entry della raccolta è stato impedito di scrivere
            'raccolte con un prodotto caricato su magazzino e altri no, quindi quando sono qui la raccolta non avrà mai il movimento di carico
            dtMagazzino = dalFabbricati.Leggi(
                piva, 0, 0,
                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                " Fabbricati.Tipo_Fabbricato_Cod = " & CostantiPersonalizzate.MAGAZZINO,
                " Fabbricato_Des ASC ",
                _objParametriServer)

            If dtMagazzino.Rows.Count = 0 Then
                Dim conferenteRagSoc = handleImprese.RagSoc_from_Piva(piva, _objParametriServer)

                Throw New GiasException(String.Format("Non esistono magazzini per l'azienda {0} (conferente/produttore), non è possibile creare carichi/scarichi di prodotto", conferenteRagSoc))
            Else
                Dim arrMagazziniCentroCampagna As DataRow() = dtMagazzino.Select("Sa_Cod = " & saCod)

                If arrMagazziniCentroCampagna.Length > 0 Then
                    idMagazzinoDest = arrMagazziniCentroCampagna(0).Field(Of Integer)("Fabbricato_Cod")
                    saCodCarico = saCod
                Else
                    idMagazzinoDest = dtMagazzino(0).Field(Of Integer)("Fabbricato_Cod")
                    saCodCarico = dtMagazzino(0).Field(Of Integer)("Sa_Cod")
                End If

            End If

            'MOVIMENTO DI CARICO
            objMovimentoC = New Movimento With {
                .Piva = piva,
                .Id_Agenda = idAgenda,
                .Sa_Cod = saCodCarico,
                .Cod_Risum = 0,
                .Cod_IndirizzoRisUm = 0,
                .Data = dataMov,
                .Lav_Cod = lavCod,
                .Cau_Mov = CAU_CARICO,
                .Mov_Desc = "Carico Produzione per Conferimento",
                .Colli = 0,
                .Scadenza_Extra = AGRODATAINIZIO,
                .Scadenza = AGRODATAFINE,
                .Ora = dataMov,
                .Extra_Date = #12/30/1899#,
                .Data_Registrazione = dataMov
            }

            objMovimentoC.Id_Mov = objSequenze.NuovoId_Tabella("Movimenti",
                                                            objMovimentoC.BaseCode,
                                                            objMovimentoC.TopCode,
                                                            _objParametriServer)

            '------------------------------------------------
            '----- MOVIMENTI DETTAGLI
            '------------------------------------------------

            'Il prodotto da assegnare al dettaglio di carico deve essere lo stesso di quello di campagna, se presente

            If matCodRaccolta = 0 Then

                'Occorre verificare che il prodotto scelto dal conferimento sia pubblico e di conseguenza sceglibile nell'azienda agricola
                matCodRaccolta = dettaglioConf.MatCod

                Dim dalMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R()
                Dim dtMatPrime As DataTable = dalMateriePrime.Leggi_da_MatCod_senzaFiltroVisibilita(dettaglioConf.ElemCod, dettaglioConf.MatCod, "", _objParametriServer)
                Dim prodottoConf = dtMatPrime.Rows(0)

                If prodottoConf.Field(Of Integer)("Sa_Cod") <> -1 Then
                    'Se il prodotto non è pubblico, devo cercarne uno compatibile da assegnare all'az agricola

                    dtMatPrime = dalMateriePrime.MateriePrime_Anagrafica(
                        piva,
                        -1,
                        dettaglioConf.ElemCod,
                        0,
                        "",
                        dettaglioConf.VegCod,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        "",
                        0,
                        "",
                        False,
                        "",
                        "",
                        _objParametriServer,
                        _objParametriUtenti)

                    If dtMatPrime.Rows.Count > 0 Then
                        Dim arrProdVarieta As DataRow() = dtMatPrime.Select("Cul_Cod = " & dettaglioConf.CulCod)

                        If arrProdVarieta.Length > 0 Then
                            'Recupero il primo prodotto per specie-varietà
                            matCodRaccolta = arrProdVarieta(0).Field(Of Integer)("Mat_Cod")
                        Else
                            'Recupero il primo prodotto per specie
                            matCodRaccolta = dtMatPrime(0).Field(Of Integer)("Mat_Cod")
                        End If
                    Else
                        'Creo un nuovo prodotto partendo da specie-varietà
                        Dim Piva_Creazione As String = _objParametriServer.PivaSuperUser 'azienda che lo crea
                        Dim Sa_Cod_Creazione As Integer = PRIVATO
                        Dim Cul_Cod As Integer = New Cultivar_R().CulCod_Altre_from_VegCod(dettaglioConf.VegCod, _objParametriServer)
                        If Cul_Cod < 1 Then
                            Throw New Exception("non c'è la varietà altre")
                        End If
                        Dim objImportaGias As New AgronicaCoreAnagrafeBIZ.Importa_GIAS
                        Dim Regolamento As enum_Cod_Regolamento = enum_Cod_Regolamento.Regolamento_Nessuno
                        Dim Flag_Biologico As Boolean = False

                        Dim Cod_Articolo As String = "RACC/" & Right("000" & dettaglioConf.VegCod, 3) &
                                    "/" & Right("00000000" & CStr(Cul_Cod), 8)

                        Dim dalSpecieVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R()

                        Dim matDes = dalSpecieVeg.VegDes_from_VegCod(dettaglioConf.VegCod, _objParametriServer) & " - Altre"

                        Dim objCore_XML_Anagrafe As New AgronicaCoreXML.XML_Anagrafe
                        Dim objCore_MP_W As New AgronicaCoreAnagrafeBIZ.Materie_Prime_W
                        Dim Flag_Insert As Boolean
                        Dim Basecode As Integer = 0
                        Dim Topcode As Integer = 200000000
                        'AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(Basecode,
                        '                                                       Topcode,
                        '                                                       Session("ASG_ProgressivoGIAS"))

                        objImportaGias.Creazione_Automatica_TrasformatoVegetale(
                            _objParametriServer,
                            objCore_XML_Anagrafe,
                            objCore_MP_W,
                            Flag_Insert,
                            matCodRaccolta,
                            Basecode,
                            Topcode,
                            Piva_Creazione,
                            Sa_Cod_Creazione,
                            Cod_Articolo,
                            matDes,
                            dettaglioConf.VegCod,
                            Cul_Cod,
                            Regolamento,
                            Flag_Biologico)

                        If Not Flag_Insert Then
                            Dim conferenteRagSoc = handleImprese.RagSoc_from_Piva(piva, _objParametriServer)

                            Throw New GiasException(String.Format(
                                                    "Il prodotto scelto non è disponibile nell'azienda {0} (conferente/produttore) " &
                                                    "e non è stato possibile crearne uno in automatico", conferenteRagSoc))
                        End If

                    End If

                End If

            End If

            'Leggo l'impostazione 181 per capire se il lotto è gestito sull'azienda agricola
            Dim lottoRaccolta = RaccolteOttieniLotto(dettaglioConf, dataMov)

            objMovDettagliC = New Movimento_Dettaglio With {
                .Piva = piva,
                .Sa_Cod = saCodCarico,
                .Id_Agenda = 0, 'Non serve specificarlo qua perché la funzione di scrittura sovrascrive questo valore con quello dell'oggetto Movimento
                .Id_Mov = 0, 'Non serve specificarlo qua perché la funzione di scrittura sovrascrive questo valore con quello dell'oggetto Movimento
                .Lav_Cod = lavCod,
                .Elem_Cod = CInt(dettaglioConf.ElemCod),
                .Pro_Cod = 0,
                .Mat_Cod = matCodRaccolta,
                .Mov_Det_Des = "",
                .Qta = qta,
                .Cal_Cod = 0, 'Questo dato non viene più valorizzato nelle raccolte
                .Extra_Int = 0,
                .Udm_Cod = dettaglioConf.DB_Udm_Cod,
                .Jolly_Int = 0,
                .Contabilizzato = NONCONTABILE,
                .Pendente = enum_Pendenza.MovGiustificato,
                .Lotto = lottoRaccolta,
                .Cod_Progetto = 0, 'In questo caso non specifico mai il riferimento all'esercizio
                .Tara = 0,
                .Udm_Cod_Extra = 0,
                .Qta_Extra = 0,
                .Ordine_Det = 0,
                .Validita_Inizio = dataMov,
                .Validita_Fine = AGRODATAFINE
            }

            objMovDettagliC.Id_Mov_Det = objSequenze.NuovoId_Tabella("Movimenti_Dettagli",
                                                            objMovDettagliC.BaseCode,
                                                            objMovDettagliC.TopCode,
                                                            _objParametriServer)

            '------------------------------------------------
            '----- MOVIMENTI DESTINAZIONI
            '------------------------------------------------

            Dim objMovDestinazioniC = New Movimento_Destinazione With {
                .Piva = piva,
                .Sa_Cod = saCodCarico,
                .Id_Agenda = 0,
                .Id_Mov = 0,
                .Id_Mov_Det = 0,
                .Appezza = 0,
                .Tipo = CostantiPersonalizzate.MAGAZZINO,
                .Id_Destinazione = idMagazzinoDest,
                .Qta = qta
            }

            'Aggancio la destinazione al dettaglio
            objMovDettagliC.Movimenti_Destinazioni = New List(Of Movimento_Destinazione) From {objMovDestinazioniC}

            'Aggancio i movimenti dettagli al movimento
            objMovimentoC.Movimenti_Dettagli = New List(Of Movimento_Dettaglio) From {objMovDettagliC}

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objMovimentoC

    End Function

    Private Function RaccolteOttieniLotto(ByRef dettaglioConf As Contabilita_Riga, ByVal dataMovRaccolta As Date) As String

        Const nomeRoutine = "ContabilitaHelper_Dettaglio.RaccolteOttieniLotto()"

        Dim lottoRaccolta As String = ""

        Dim impreseImpostazioniR As New Imprese_Impostazioni_R

        Try
            If _raccolteImpLotto Is Nothing Then
                'Leggo l'impostazione solo una volta
                _raccolteImpLotto = impreseImpostazioniR.LeggiScalareMulticentroAziendaSuperUser_ElemCod(dettaglioConf.PivaConferente,
                    Nothing,
                    enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI,
                    CInt(dettaglioConf.ElemCod),
                    enum_Gestione_Lotti.Facoltativa,
                    _objParametriUtenti,
                    _objParametriServer)
            End If

            'Se nessuna → stringa vuota
            'Se facoltativa → dal conferimento se c'è
            'Se obbligatoria → dal conferimento se c'è, altrimenti default yyyyMMdd (concorde con il QdC)
            Select Case _raccolteImpLotto
                Case enum_Gestione_Lotti.Nessuna
                    lottoRaccolta = ""

                Case enum_Gestione_Lotti.Facoltativa
                    lottoRaccolta = If(dettaglioConf.Lotto, "")

                Case enum_Gestione_Lotti.Obbligatoria
                    lottoRaccolta = If(Not String.IsNullOrWhiteSpace(dettaglioConf.Lotto), dettaglioConf.Lotto, dataMovRaccolta.ToString("yyyyMMdd"))

            End Select


        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return lottoRaccolta

    End Function

    Private Function ScriviGHGRegistazioneCollegata(ByVal objGHG_Registrazioni As List(Of GHG_Registrazione),
                                                    ByVal objContabDettaglio As Contabilita_Riga,
                                                    ByVal Data_Movimento As DateTime,
                                                    ByVal Des_Lib As String,
                                                    ByVal Lav_Cod As Integer,
                                                    ByVal Id_Agenda_GHG As Integer) As Integer

        Const nomeRoutine = "ContabilitaHelper.ScriviGHGRegistazioneCollegata()"
        Dim ObjAgenda As Operazione_Agenda
        Dim objGHG_RegistrazioniHelper As New Agenda_GHG_Registrazione_Helper
        Dim Dummy As Boolean
        Dim Id_GHG_Registrazioni As Integer
        Dim bOk As Boolean

        Try

            For i = 0 To objGHG_Registrazioni.Count - 1

                'Solo la prima volta
                If i = 0 Then

                    Select Case Id_Agenda_GHG

                        Case 0

                            '------------------------------------------------
                            '----- AGENDA
                            '------------------------------------------------

                            ObjAgenda = New Operazione_Agenda With {
                            .Tipo_Operazione = enum_TipoOperazioneDB.Scrittura,
                            .Id_Agenda = 0,
                            .Data = Data_Movimento,
                            .Piva = objContabDettaglio.Piva,
                            .Sa_Cod = objContabDettaglio.DestinazioneCarico.SaCod,
                            .Lav_Cod = LAVCOD_GHG,
                            .Linea_Cod = 0,
                            .Preparazione_Cod = 0,
                            .Id_Trasformazione = 0,
                            .Des_Lib = "GHG Registrazioni associati al " & Des_Lib & " Lotto " & If(objContabDettaglio.Lotto, "")
                              }


                            'Scrittura
                            If Not IsNothing(ObjAgenda) Then
                                Dim objAgendaHelper As New Agenda_Operazione_Helper
                                Id_Agenda_GHG = objAgendaHelper.Scrivi(ObjAgenda, _objParametriServer, flagUsaOraReale:=True)


                                '------------------------------------------------
                                '----- GHG REGISTRAZIONI
                                '------------------------------------------------                                           

                                Dim objGHG_Registrazione = New GHG_Registrazione With {
                                .PivaSuperUser = _objParametriServer.PivaSuperUser,
                                .Piva = objContabDettaglio.Piva,
                                .Id_Agenda_GHG = Id_Agenda_GHG,
                                .Direttiva_Cod = 1,
                                .Elem_Cod = objContabDettaglio.ElemCod,
                                .Pro_Cod = objContabDettaglio.ProCod,
                                .Mat_Cod = objContabDettaglio.MatCod,
                                .Lotto = objContabDettaglio.Lotto
                                 }

                                'Scrittura Record Vuoto (in fondo alla funzione ci sono gli update puntuali)
                                objGHG_RegistrazioniHelper = New Agenda_GHG_Registrazione_Helper
                                Id_GHG_Registrazioni = objGHG_RegistrazioniHelper.Scrivi(objGHG_Registrazione, _objParametriServer)


                                '------------------------------------------------
                                '----- RIFERIMENTO OPERAZIONI DI AGENDA
                                '------------------------------------------------                       

                                Dim objRiferimento = New Movimento_Dettaglio_Riferimento With {
                                    .Piva = objContabDettaglio.Piva,
                                    .Sa_Cod = objContabDettaglio.DestinazioneCarico.SaCod,
                                    .Id_Agenda = objContabDettaglio.IdAgenda,
                                    .Id_Mov = objContabDettaglio.IdMov,
                                    .Id_Mov_Det = objContabDettaglio.IdMovDet,
                                    .Lav_Cod = Lav_Cod,
                                    .Cau_Mov = CAU_REGISTRAZIONI,
                                    .Piva_Rif = objContabDettaglio.Piva,
                                    .Sa_Cod_Rif = objContabDettaglio.DestinazioneCarico.SaCod,
                                    .Id_Agenda_Rif = Id_Agenda_GHG,
                                    .Id_Mov_Rif = 0,
                                    .Id_Mov_Det_Rif = 0,
                                    .Lav_Cod_Rif = LAVCOD_GHG,
                                    .Cau_Mov_Rif = CAU_REGISTRAZIONI,
                                    .Tipo_Associazione = 0
                                     }

                                Dim objRiferimentoHelper As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                                Dummy = objRiferimentoHelper.Scrivi(objRiferimento, _objParametriServer)


                            End If


                        Case Else

                            'Modifica Campi di 'Prodotto' Tabella GHG_Registrazioni
                            objGHG_RegistrazioniHelper.ModificaProdotto(objContabDettaglio.Piva,
                                                                        Id_Agenda_GHG,
                                                                        Id_GHG_Registrazioni,
                                                                        objContabDettaglio.ElemCod,
                                                                        objContabDettaglio.ProCod,
                                                                        objContabDettaglio.MatCod,
                                                                        objContabDettaglio.Lotto,
                                                                        _objParametriServer)

                    End Select

                End If


                'Update Puntuale dei campi
                objGHG_RegistrazioniHelper.ModificaPuntuale(objContabDettaglio.Piva,
                                                           Id_Agenda_GHG,
                                                           Id_GHG_Registrazioni,
                                                           objGHG_Registrazioni(i).TipoCampo,
                                                           objGHG_Registrazioni(i).TipoDato,
                                                           objGHG_Registrazioni(i).Nome_Campo,
                                                           objGHG_Registrazioni(i).Valore_Des,
                                                           objGHG_Registrazioni(i).ID_Indice_Det,
                                                           objGHG_Registrazioni(i).Elenco_Val,
                                                           _objParametriServer)

            Next i


            'Aggiornamento GHG Totale
            bOk = objGHG_RegistrazioniHelper.AggiornaGHGTotal(objContabDettaglio.Piva,
                                                              Id_Agenda_GHG,
                                                              _objParametriServer)
        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return Id_Agenda_GHG

    End Function


    Public Function CancellaSingolaRigaDocumento(ByVal objContabDettaglio As Contabilita_Riga,
                                                 ByVal lavCod As Integer,
                                                 ByVal cauMov As String,
                                                 ByVal idMovT As Integer,
                                                 ByRef idMovCS As Integer,
                                                 ByVal dataOp As Date,
                                                 ByVal usernameOperatore As String,
                                                 ByVal messaggioDettagliato As Boolean
                                                 ) As Contabilita_Output

        Const nomeRoutine = "ContabilitaHelper.CancellaSingolaRigaDocumento()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim idAgenda As Integer = 0
        Dim desLib As String = ""
        Dim idMovSec As Integer = 0
        Dim idMovDet As Integer = 0
        Dim xRisp As Boolean = False
        Dim msgError As String = ""
        Dim flagOggettoCorretto As Boolean

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyRigaContabilita(objContabDettaglio, enum_TipoOperazioneDB.Cancellazione, _objParametriServer)

            If flagOggettoCorretto Then

                'TODO: fare tutto il controllo preventivo sulla cancellazione riga

                'TODO: fare tutta la parte di aggiornamento delle giacenze

                Dim objMovDetHelper As New Agenda_Movimenti_Dettagli_Helper
                xRisp = objMovDetHelper.Cancella(objContabDettaglio.Piva,
                                                 objContabDettaglio.SaCod,
                                                 objContabDettaglio.IdAgenda,
                                                 objContabDettaglio.IdMov,
                                                 objContabDettaglio.IdMovDet,
                                                 _objParametriServer)



                'TODO: Devo aggiornare in testata (4000) alcuni valori
                '- Leggo oggetto e poi modifico --> lettura inutile dell'obj con figli
                '- Dovrei fare funzione ad hoc che passa il valore da aggiungere e lascia a sql fare la somma --> ok per inserimento, no per modifica
                '- Dal ad hoc per leggere il solo movimento e salvarmi tutti i campi che devo contro-aggiornare
                '    (alla fine prendo questi valori e +/- differenza con nuovi valori, poi update puntuale di 4000)


            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return New Contabilita_Output With {
            .Risultato = xRisp,
            .MsgError = msgError,
            .Id_Agenda = idAgenda,
            .Id_Mov_Testata = idMovT,
            .Id_Mov_Secondario = idMovSec,
            .Des_Lib = desLib,
            .Id_Mov_Carico = idMovCS,
            .Id_Mov_Scarico = idMovCS,
            .Id_Mov_Det = idMovDet
        }

    End Function

    Public Function ScriviSingolaRigaDocumento(ByVal objContabDettaglio As Contabilita_Riga,
                                               ByVal lavCod As Integer,
                                               ByVal cauMov As String,
                                               ByVal idMovT As Integer,
                                               ByRef idMovCS As Integer,
                                               ByVal dataOp As Date,
                                               ByVal usernameOperatore As String,
                                               ByVal messaggioDettagliato As Boolean,
                                               Optional ByVal objContabTestata As Contabilita_Testata = Nothing
                                               ) As Contabilita_Output

        Const nomeRoutine = "ContabilitaHelper.ScriviSingolaRigaDocumento()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim objDettaglio As Movimento_Dettaglio
        Dim idAgenda As Integer = 0
        Dim desLib As String = ""
        Dim numDoc As Integer = 0
        Dim numDocVisualizzato As String = ""
        Dim idMovSec As Integer = 0
        Dim idMovDet As Integer = 0
        Dim movMagazzino As Contabilita_Chiave_Mov = Nothing
        Dim conteggiTotali As Contabilita_Totali_Testata = Nothing
        Dim xRisp As Boolean = False
        Dim msgError As String = ""
        Dim flagOggettoCorretto As Boolean
        Dim Progressivo As Integer

        Try
            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            flagOggettoCorretto = CheckPropertyRigaContabilita(objContabDettaglio, enum_TipoOperazioneDB.Scrittura, _objParametriServer)

            If flagOggettoCorretto Then

                idAgenda = objContabDettaglio.IdAgenda

                If idAgenda = 0 OrElse idMovT = 0 Then

                    If objContabTestata Is Nothing Then
                        Throw New Exception("objContab Testata Is Nothing ma idMovT = 0")
                    End If

                    'TODO: è la prima riga in assoluto, quindi devo scrivere anche la testata
                    Dim helperTestata As New ContabilitaHelper_Testata(_objParametriServer, _objParametriUtenti)
                    Dim test As Contabilita_Output = helperTestata.ScriviTestataDocumento(objContabTestata, messaggioDettagliato)

                    If test Is Nothing OrElse test.Risultato = False OrElse test.Id_Agenda = 0 OrElse test.Id_Mov_Testata = 0 Then
                        Throw New Exception(String.Format("Impossibile salvare la testata del documento [{0}]", test.MsgError))
                    End If

                    objContabDettaglio.IdAgenda = test.Id_Agenda
                    idAgenda = test.Id_Agenda
                    idMovT = test.Id_Mov_Testata
                    idMovSec = test.Id_Mov_Secondario
                    desLib = test.Des_Lib
                    numDoc = test.Doc_Numero
                    numDocVisualizzato = test.Doc_Numero_Visualizzato

                End If

                If idMovCS = 0 Then

                    'è la prima riga in assoluto, quindi prima di proseguire devo creare il movimento 7300/7350
                    Dim objMovT = UtilityHelper.CercaMovimentoSpecificoSenzaDettagli(CAU_REGISTRAZIONI,
                                                                                     objContabDettaglio.Piva,
                                                                                     idAgenda,
                                                                                     _objParametriServer)

                    'MOVIMENTO DI CARICO/SCARICO
                    Dim objMovimentoCS = New Movimento With {
                        .Piva = objMovT.Piva,
                        .Sa_Cod = 0,
                        .Id_Agenda = objMovT.Id_Agenda,
                        .Id_Mov = 0,
                        .Lav_Cod = objMovT.Lav_Cod,
                        .Cau_Mov = cauMov,
                        .Data = objMovT.Data,
                        .Scadenza = AGRODATAFINE,
                        .Doc_Numero_Sin = objMovT.Doc_Numero_Sin,
                        .Doc_Numero = objMovT.Doc_Numero,
                        .Doc_Numero_Des = objMovT.Doc_Numero_Des,
                        .Cod_Risum = objMovT.Cod_Risum,
                        .Cod_IndirizzoRisUm = objMovT.Cod_IndirizzoRisUm,
                        .Cod_Destinazione = objMovT.Cod_Destinazione,
                        .Cod_IndirizzoDestinazione = objMovT.Cod_IndirizzoDestinazione,
                        .Cod_Vettore = objMovT.Cod_Vettore,
                        .Cod_IndirizzoVettore = objMovT.Cod_IndirizzoVettore,
                        .Cod_RisUm_Aggiuntivo = objMovT.Cod_RisUm_Aggiuntivo,
                        .Cod_Indirizzo_Aggiuntivo = objMovT.Cod_Indirizzo_Aggiuntivo,
                        .Ora = objMovT.Ora,
                        .Extra_Date = #12/30/1899#,
                        .Scadenza_Extra = AGRODATAINIZIO
                    }

                    '.Mov_Desc = CreaMovDescLavorazione(cauMov, tipoLavorazioneDes),
                    '.Username_Creazione = objContabDettaglio

                    Dim objSequenze As New Agro_Sequenze
                    Dim idMovCSTemp As Integer = objSequenze.NuovoId_Tabella("Movimenti", 0, 200000000, _objParametriServer)
                    objMovimentoCS.Id_Mov = idMovCSTemp

                    Dim objMovHelper As New Agenda_Movimenti_Helper
                    Dim xRispCS As Boolean = objMovHelper.Scrivi(objMovimentoCS, _objParametriServer, flagUsaOraReale:=True)

                    If xRispCS = True Then
                        movMagazzino = New Contabilita_Chiave_Mov With {
                            .Piva = objMovimentoCS.Piva,
                            .IdAgenda = objMovimentoCS.Id_Agenda,
                            .SaCod = objMovimentoCS.Sa_Cod,
                            .IdMov = objMovimentoCS.Id_Mov,
                            .CauMov = objMovimentoCS.Cau_Mov
                        }
                        idMovCS = movMagazzino.IdMov
                        objContabDettaglio.IdMov = idMovCS
                    Else
                        idMovCS = 0
                        Throw New Exception(String.Format("Non è stato possibile creare l'oggetto Movimento di Carico/Scarico [{0}]", cauMov))
                    End If
                Else
                    movMagazzino = New Contabilita_Chiave_Mov With {
                        .Piva = objContabDettaglio.Piva,
                        .IdAgenda = objContabDettaglio.IdAgenda,
                        .SaCod = objContabDettaglio.SaCod,
                        .IdMov = idMovCS,
                        .CauMov = cauMov
                    }
                    objContabDettaglio.IdMov = idMovCS
                End If

                'TODO: se ho più di una riga devo spezzare!!!
                Dim objOutputSpacca As Contabilita_Output = Nothing
                Dim flagSpacca As Boolean = False
                If objContabDettaglio.Confezionamenti IsNot Nothing AndAlso objContabDettaglio.Confezionamenti.Count > 1 Then

                    flagSpacca = True
                    objOutputSpacca = SpaccaRigheConfezionamentiDiversi(objContabDettaglio,
                                                                        lavCod, cauMov,
                                                                        idMovT, idMovCS,
                                                                        dataOp, usernameOperatore,
                                                                        messaggioDettagliato, msgError, objContabTestata)

                    If objOutputSpacca IsNot Nothing AndAlso objOutputSpacca.Risultato = True Then
                        xRisp = True
                        idMovDet = objOutputSpacca.Id_Mov_Det
                        conteggiTotali = objOutputSpacca.ConteggiTotali
                    Else
                        xRisp = False
                        msgError &= objOutputSpacca.MsgError
                    End If

                Else

                    'Ho singola riga confezionamenti o nessuna ==> proseguo con la scrittura vera e propria

                    objDettaglio = GeneraRigaContabilita(objContabDettaglio,
                                                         lavCod,
                                                         cauMov,
                                                         idMovT,
                                                         idMovCS,
                                                         dataOp,
                                                         usernameOperatore)

                    If objDettaglio Is Nothing Then
                        Throw New Exception("Non è stato possibile creare l'oggetto Riga Dettaglio")
                    End If

                    Progressivo = 0
                    Dim objDetHelper As New Agenda_Movimenti_Dettagli_Helper
                    idMovDet = objDetHelper.Scrivi(objDettaglio, _objParametriServer,,, Progressivo)

                    If Progressivo <> 0 Then
                        objContabDettaglio.CalCod = Progressivo
                    End If


                    If idMovDet > 0 Then

                        objContabDettaglio.IdMovDet = idMovDet

                        If UtilityHelper.IsTrasformatoFF(objContabDettaglio.ModuloGias, objContabDettaglio.ElemCod) Then

                            UtilityHelper.SistemaImballaggiDocumento(objContabDettaglio.Piva, idAgenda, idMovCS,
                                                                     enum_TipoOperazioneDB.Scrittura, _objParametriServer,
                                                                     objContabDettaglio.ModuloGias, objDettaglio)

                        End If


                        'TODO: aggiorno colli testata + peso lordo testata (+ eventualmente provvigione, castelletto, ecc...)
                        conteggiTotali = UtilityHelper.AggiornaConteggiTotali(objContabDettaglio.Piva, lavCod,
                                                                              idAgenda, _objParametriServer, idMovT)

                        xRisp = True

                        'Scrittura Raccolte collegate a Conferimento: se create automaticamente hanno Tipo_Associazione impostato a zero,
                        'altrimenti se pre-esistenti è impostato ad uno
                        If objContabDettaglio.Tipo_Associazione = 0 AndAlso
                           UtilityHelper.IsAccettazione(lavCod) = True AndAlso
                           UtilityHelper.IsTrasformatoFF(objContabDettaglio.ModuloGias, objContabDettaglio.ElemCod) AndAlso
                           objContabDettaglio.PivaConferente <> "" Then

                            'Modifica del 10/06/2022: Tolgo la possibilità di scrivere la raccolta con impianti indefiniti
                            If (Not IsNothing(objContabDettaglio.RigheImpianti) AndAlso objContabDettaglio.RigheImpianti.Count > 0) Then

                                Dim outpRaccolteAutomatiche = ScriviRaccolteCollegate(objContabDettaglio, objContabTestata, lavCod, cauMov, dataOp, enum_TipoOperazioneDB.Scrittura)

                                If outpRaccolteAutomatiche.Risultato = True Then
                                    xRisp = True
                                Else
                                    xRisp = False
                                    msgError &= outpRaccolteAutomatiche.MsgError
                                End If

                            End If

                        End If



                        'Scrittura Tabella GHG_Registrazioni
                        If objContabDettaglio.ElemCod <> RIGA_DESCRIZIONE_LIBERA And objContabDettaglio.ElemCod <> ALTRI_BENI Then

                            If (Not IsNothing(objContabDettaglio.GHG_Registrazioni) AndAlso objContabDettaglio.GHG_Registrazioni.Count > 0) Then

                                Dim Id_Agenda_GHG As Integer = ScriviGHGRegistazioneCollegata(objContabDettaglio.GHG_Registrazioni, objContabDettaglio, dataOp, objContabTestata.DescrizioneAgenda, lavCod, 0)

                                If Id_Agenda_GHG > 0 Then
                                    xRisp = True
                                Else
                                    xRisp = False
                                    msgError &= "Si è verificato un errore durante il salvataggio dei parametri GHG"
                                End If

                            End If

                        End If


                        If objContabDettaglio.Tipo_Associazione = 1 AndAlso
                               UtilityHelper.IsAccettazione(lavCod) = True AndAlso
                               UtilityHelper.IsTrasformatoFF(objContabDettaglio.ModuloGias, objContabDettaglio.ElemCod) AndAlso
                               objContabDettaglio.PivaConferente <> "" Then

                            Dim outputAggiornaRaccolte = AggiornaRaccoltaAziendaCampagna(objContabDettaglio, objContabTestata, enum_TipoOperazioneDB.Scrittura)

                            If outputAggiornaRaccolte.Risultato = True Then
                                xRisp = True
                            Else
                                xRisp = False
                                msgError &= outputAggiornaRaccolte.MsgError
                            End If

                        End If

                        'TODO: aggiornare se sono cambiati Des_Lib e Data_Movimento
                        Dim objAgronicaLogAgendaW As New AgronicaCoreContabDAL.AgronicaLogAgenda_W
                        objAgronicaLogAgendaW.Scrivi(dataOp,
                                                     enum_TipoOperazioneDB.Modifica,
                                                     desLib,
                                                     idAgenda,
                                                     objContabDettaglio.Piva,
                                                     0,
                                                     lavCod,
                                                     CInt(enum_Id_Servizio.GiasOnline),
                                                     _objParametriServer)


                        If UtilityHelper.IsContrattoAffitto(lavCod) Then

                            'In inserimento di una nuova riga dettaglio, devo inserirere tutti i riferimenti catastali presenti
                            'anche per la riga appena inserita

                            Dim scriviContrattiImpreseParticelle As New AgronicaCoreContabBIZ.ContrattiXImpreseXParticelle(_objParametriServer)

                            scriviContrattiImpreseParticelle.InserisciContrattiImpreseParticelleNuovaRiga(objContabDettaglio.Piva,
                                                                                                          idAgenda,
                                                                                                          idMovDet)

                        End If

                    End If

                End If

            End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return New Contabilita_Output With {
            .Risultato = xRisp,
            .MsgError = msgError,
            .Id_Agenda = idAgenda,
            .Id_Mov_Testata = idMovT,
            .Id_Mov_Secondario = idMovSec,
            .Des_Lib = desLib,
            .Doc_Numero = numDoc,
            .Doc_Numero_Visualizzato = numDocVisualizzato,
            .MovimentoMagazzino = movMagazzino,
            .Id_Mov_Carico = idMovCS,
            .Id_Mov_Scarico = idMovCS,
            .Id_Mov_Det = idMovDet,
            .ConteggiTotali = conteggiTotali
        }

    End Function

    Private Function GeneraRigaContabilita(ByVal objContabDettaglio As Contabilita_Riga,
                                           ByVal lavCod As Integer,
                                           ByVal cauMov As String,
                                           ByVal idMovT As Integer,
                                           ByVal idMovCS As Integer,
                                           ByVal dataOp As Date,
                                           ByVal usernameOperatore As String,
                                           Optional ByVal movDetPrecedente As Movimento_Dettaglio = Nothing
                                           ) As Movimento_Dettaglio

        Const nomeRoutine = "ContabilitaHelper.GeneraRigaContabilita()"
        Dim objDettaglio As Movimento_Dettaglio

        Try

            'movDetPrecedente mi serve per impostare le chiavi quando sono in realtà in modifica della riga

            'TODO: se non è prima riga devo anche aggiornare i contatori sul movimento di intestazione (num colli, peso totale, ...)

            AggiustaUdmPerScrittura(objContabDettaglio, lavCod)

            'TODO: DEBUG!!!! Devo passare da AggiornaDettagliEconomici altrimenti non ho il prezzo/kg
            UtilityHelper.AggiornaDettagliEconomici(objContabDettaglio, lavCod)


            'Faccio una serie di aggiustamenti/ricalcoli
            SistemaValoriRigaPerScrittura(objContabDettaglio, lavCod)

            Dim magazzino As New Contabilita_Magazzino

            Select Case cauMov
                Case CAU_SCARICO
                    magazzino = objContabDettaglio.DestinazioneScarico
                Case CAU_CARICO
                    magazzino = objContabDettaglio.DestinazioneCarico
                Case Else
                    If objContabDettaglio.jolly_int = MagazzinoMovimentato Then
                        Throw New Exception(String.Format("Il Cau_Mov [{0}] non è gestito correttamente per ricavare il magazzino.", cauMov))
                    End If
            End Select


            '------------------------------------------------
            '----- MOVIMENTO DETTAGLIO
            '------------------------------------------------
            'TODO: ricavare mov_det_des
            Dim movDetDes As String = objContabDettaglio.MatDes

            If objContabDettaglio.ElemCod = RIGA_DESCRIZIONE_LIBERA OrElse objContabDettaglio.ElemCod = ALTRI_BENI Then
                movDetDes = objContabDettaglio.BeniStrumentali
            End If

            Dim w_RicCod As Integer
            Dim w_Contabilizzato As Integer

            If UtilityHelper.IsMovimentoMagazzino(lavCod) Or UtilityHelper.IsContrattoAffitto(lavCod) Then
                w_Contabilizzato = NONCONTABILE
                w_RicCod = 0
            Else
                w_Contabilizzato = CONTABILE
                w_RicCod = 2
            End If

            objDettaglio = New Movimento_Dettaglio With {
                .Piva = objContabDettaglio.Piva,
                .Sa_Cod = If(magazzino.SaCod, 0),
                .Id_Agenda = objContabDettaglio.IdAgenda,
                .Id_Mov = idMovCS,
                .Id_Mov_Det = If(movDetPrecedente IsNot Nothing, movDetPrecedente.Id_Mov_Det, 0),
                .Data = dataOp,
                .Validita_Inizio = dataOp,
                .Lav_Cod = lavCod,
                .Cau_Mov = cauMov,
                .Ordine_Det = If(movDetPrecedente IsNot Nothing, movDetPrecedente.Ordine_Det, objContabDettaglio.OrdineDet),
                .Elem_Cod = objContabDettaglio.ElemCod,
                .Pro_Cod = objContabDettaglio.ProCod,
                .Mat_Cod = objContabDettaglio.MatCod,
                .Mov_Det_Des = movDetDes,
                .Mat_Cod_Alias = If(objContabDettaglio.MatCodAlias.HasValue, objContabDettaglio.MatCodAlias.Value, 0),
                .Cal_Cod = objContabDettaglio.CalCod,
                .Cod_Progetto = If(objContabDettaglio.Cod_Progetto, 0),
                .Udm_Cod = objContabDettaglio.DB_Udm_Cod,
                .Qta = objContabDettaglio.DB_Qta,
                .Extra_Int = If(objContabDettaglio.ExtraInt, 0),
                .Udm_Cod_Extra = objContabDettaglio.DB_Udm_Cod_Extra,
                .Qta_Extra = objContabDettaglio.DB_Qta_Extra,
                .Qta_Extra_Totale = objContabDettaglio.KgNetti,
                .Tara = objContabDettaglio.Tara,
                .Jolly_Int = objContabDettaglio.jolly_int,
                .Cod_Iva = objContabDettaglio.CodIva,
                .Iva = objContabDettaglio.Iva,
                .ChkIva_Manuale = If(objContabDettaglio.ForzaIva = True, 1, 0),
                .Lotto = objContabDettaglio.Lotto,
                .Variazione = objContabDettaglio.Degrado,
                .Prezzo_Unitario = objContabDettaglio.DB_Prezzo_Unitario,
                .Prezzo_Unitario_Netto = objContabDettaglio.DB_Prezzo_Unitario_Netto,
                .Prezzo_Effettivo = objContabDettaglio.PrezzoEffettivoKgL,
                .Prezzo_Livello = objContabDettaglio.PrezzoRiferitoA,
                .TempoCarenza = objContabDettaglio.ValoreRiferimentoPrezzo,
                .Sconto_Modalita = objContabDettaglio.ScontoModalita,
                .Sconto = objContabDettaglio.ScontoMaggiorazioneBase,
                .Sconto_Listino = objContabDettaglio.ScontoAddizTotalePerc,
                .Sconto_Testo = objContabDettaglio.ScontoAddizTesto,
                .Imponibile = objContabDettaglio.ImponibileTotale,
                .Imponibile_Netto = objContabDettaglio.ImponibileTotaleNetto,
                .Contabilizzato = w_Contabilizzato,
                .Anno = If(objContabDettaglio.Anno, dataOp.Year),
                .Ric_Cod = w_RicCod,
                .Cod_Conto = objContabDettaglio.ContoEconomico,
                .Ric_Cod_Pat = w_RicCod,
                .Cod_Conto_Pat = objContabDettaglio.ContoPatrimoniale,
                .Pendente = objContabDettaglio.Pendente,
                .Qta_Dettaglio1 = objContabDettaglio.NumContenitori,
                .Qta_Dettaglio2 = objContabDettaglio.NumImballi,
                .Rif_Esterno = If(movDetPrecedente IsNot Nothing, movDetPrecedente.Rif_Esterno, If(objContabDettaglio.Rif_Esterno, "")),
                .Rif_Esterno_2 = If(movDetPrecedente IsNot Nothing, movDetPrecedente.Rif_Esterno_2, If(objContabDettaglio.Rif_Esterno_2, "")),
                .Extra_Str = objContabDettaglio.Extra_Str,
                .Username_Modifica = usernameOperatore
            }

            If movDetPrecedente IsNot Nothing Then
                objDettaglio.Data_Creazione = movDetPrecedente.Data_Creazione
                objDettaglio.Username_Creazione = movDetPrecedente.Username_Creazione
            Else
                objDettaglio.Username_Creazione = usernameOperatore
            End If

            '------------------------------------------------
            '----- MOVIMENTO DESTINAZIONE
            '------------------------------------------------
            'Il jolly_int potrebbe essere magazzino non movimentato, ma ho cmq selezionato magazzino (vedi ordine)
            If magazzino.SaCod IsNot Nothing AndAlso magazzino.SaCod <> 0 And Not UtilityHelper.IsContrattoAffitto(lavCod) Then

                Dim objDestinazione As New Movimento_Destinazione With {
                    .Piva = objDettaglio.Piva,
                    .Sa_Cod = If(magazzino.SaCod, 0),
                    .Id_Agenda = objDettaglio.Id_Agenda,
                    .Id_Mov = objDettaglio.Id_Mov,
                    .Id_Mov_Det = objDettaglio.Id_Mov_Det,
                    .Appezza = If(magazzino.Appezza, 0),
                    .Id_Destinazione = If(magazzino.IdDestinazione, 0),
                    .Tipo = If(magazzino.TipoDestinazione, 0),
                    .Data = objDettaglio.Data,
                    .Qta = objDettaglio.Qta,
                    .Qta2 = objDettaglio.Qta,
                    .Qta_Dest1 = objContabDettaglio.NumContenitori,
                    .Qta_Dest2 = objContabDettaglio.NumImballi,
                    .Username_Modifica = usernameOperatore
                }

                If movDetPrecedente IsNot Nothing AndAlso
                   movDetPrecedente.Movimenti_Destinazioni IsNot Nothing AndAlso
                   movDetPrecedente.Movimenti_Destinazioni.Count > 0 Then
                    objDestinazione.Data_Creazione = movDetPrecedente.Movimenti_Destinazioni(0).Data_Creazione
                    objDestinazione.Username_Creazione = movDetPrecedente.Movimenti_Destinazioni(0).Username_Creazione
                Else
                    objDestinazione.Username_Creazione = usernameOperatore
                End If

                'Aggancio il movimento destinazione sul movimento dettaglio
                objDettaglio.Movimenti_Destinazioni = New List(Of Movimento_Destinazione) From {
                    objDestinazione
                }

            End If

            '------------------------------------------------
            '----- MOVIMENTO DETTAGLIO TECNICO
            '------------------------------------------------
            'TODO: Movimento_Dettaglio_Tecnico

            Dim scriviMovDettTecnico As Boolean = False

            If cauMov = CAU_CARICO Then
                Select Case True

                    Case objContabDettaglio.ElemCod = CostantiPersonalizzate.FERTILIZZANTI
                        scriviMovDettTecnico = True

                    Case objContabDettaglio.ElemCod = CostantiPersonalizzate.FORMULATI AndAlso
                         objContabDettaglio.PuaRegolamento = Tipo_Regolamento_Bio
                        scriviMovDettTecnico = True

                End Select
            End If

            If scriviMovDettTecnico Then

                Dim newIdRegDettaglio As Integer = 0
                If movDetPrecedente IsNot Nothing AndAlso
                   movDetPrecedente.Movimenti_Dettagli_Tecnici IsNot Nothing AndAlso
                   movDetPrecedente.Movimenti_Dettagli_Tecnici.Count = 1 Then
                    newIdRegDettaglio = movDetPrecedente.Movimenti_Dettagli_Tecnici(0).Id_Reg_Dettaglio
                End If

                Dim objMovTec As New Movimento_Dettaglio_Tecnico With {
                    .Piva = objDettaglio.Piva,
                    .Sa_Cod = objDettaglio.Sa_Cod,
                    .Id_Agenda = objDettaglio.Id_Agenda,
                    .Id_Mov = objDettaglio.Id_Mov,
                    .Id_Mov_Det = objDettaglio.Id_Mov_Det,
                    .Id_Reg_Dettaglio = newIdRegDettaglio,
                    .Data = objDettaglio.Data,
                    .N = objContabDettaglio.N,
                    .P = objContabDettaglio.P2O5,
                    .K = objContabDettaglio.K2O,
                    .Cu = objContabDettaglio.Cu,
                    .Extra_Int = objContabDettaglio.PuaRegolamento,
                    .Username_Modifica = usernameOperatore
                }

                If movDetPrecedente IsNot Nothing AndAlso
                   movDetPrecedente.Movimenti_Dettagli_Tecnici IsNot Nothing AndAlso
                   movDetPrecedente.Movimenti_Dettagli_Tecnici.Count > 0 Then
                    objMovTec.Data_Creazione = movDetPrecedente.Movimenti_Dettagli_Tecnici(0).Data_Creazione
                    objMovTec.Username_Creazione = movDetPrecedente.Movimenti_Dettagli_Tecnici(0).Username_Creazione
                Else
                    objMovTec.Username_Creazione = usernameOperatore
                End If

                'Aggancio il movimento dettaglio tecnico sul movimento dettaglio
                objDettaglio.Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico) From {
                    objMovTec
                }

            End If

            '------------------------------------------------
            '----- MOVIMENTO DETTAGLIO TECNICO EXTRA
            '------------------------------------------------
            If objContabDettaglio.ElemCod <> RIGA_DESCRIZIONE_LIBERA AndAlso Not UtilityHelper.IsContrattoAffitto(lavCod) Then

                Dim dettaglioConfezionamento As New Contabilita_Riga_Confezionamento
                Dim matCodContenitore As Integer = 0
                Dim matCodImballo As Integer = 0

                Dim riscontratiPesoLordo As Integer = If(objContabDettaglio.Riscontrati_PesoLordo, 0)
                Dim riscontratiPesoNetto As Integer = If(objContabDettaglio.Riscontrati_PesoNetto, 0)

                Dim riscontratiNumImballi As Integer = -1
                Dim riscontratiNumContenitori As Integer = -1
                Dim riscontratiNumConfezioni As Integer = -1

                Dim riscontratiTaraUnitImballi As Decimal = -1
                Dim riscontratiTaraUnitContenitori As Decimal = -1
                Dim riscontratiTaraUnitConfezioni As Decimal = -1

                If objContabDettaglio.Confezionamenti IsNot Nothing AndAlso
                   objContabDettaglio.Confezionamenti.Count = 1 Then

                    dettaglioConfezionamento = objContabDettaglio.Confezionamenti(0)

                    matCodContenitore = UtilityHelper.GetMatCodContenitore(dettaglioConfezionamento, objContabDettaglio.Piva, _objParametriServer)
                    matCodImballo = UtilityHelper.GetMatCodImballo(dettaglioConfezionamento, objContabDettaglio.Piva, _objParametriServer)

                    riscontratiNumImballi = If(dettaglioConfezionamento.Num_Imballi_Riscontrati, -1)
                    riscontratiTaraUnitImballi = If(dettaglioConfezionamento.Tara_Unit_Imballo_Riscontrata, -1)
                    riscontratiNumContenitori = If(dettaglioConfezionamento.Num_Colli_Riscontrati, -1)
                    riscontratiTaraUnitContenitori = If(dettaglioConfezionamento.Tara_Unit_Collo_Riscontrata, -1)
                    riscontratiNumConfezioni = If(dettaglioConfezionamento.Num_Conf_Riscontrate, -1)
                    riscontratiTaraUnitConfezioni = If(dettaglioConfezionamento.Tara_Unit_Conf_Riscontrata, -1)

                End If

                'Se almeno uno dei valori è diverso dal proprio default, anche tutti gli altri che hanno default -1 devono essere diversi da tale valore
                If riscontratiPesoLordo <> 0 OrElse
                       riscontratiPesoNetto <> 0 OrElse
                       riscontratiNumImballi <> -1 OrElse
                       riscontratiTaraUnitImballi <> -1 OrElse
                       riscontratiNumContenitori <> -1 OrElse
                       riscontratiTaraUnitContenitori <> -1 OrElse
                       riscontratiNumConfezioni <> -1 OrElse
                       riscontratiTaraUnitConfezioni <> -1 Then

                    riscontratiNumImballi = If(riscontratiNumImballi = -1, 0, riscontratiNumImballi)
                    riscontratiTaraUnitImballi = If(riscontratiTaraUnitImballi = -1, 0, riscontratiTaraUnitImballi)
                    riscontratiNumContenitori = If(riscontratiNumContenitori = -1, 0, riscontratiNumContenitori)
                    riscontratiTaraUnitContenitori = If(riscontratiTaraUnitContenitori = -1, 0, riscontratiTaraUnitContenitori)
                    riscontratiNumConfezioni = If(riscontratiNumConfezioni = -1, 0, riscontratiNumConfezioni)
                    riscontratiTaraUnitConfezioni = If(riscontratiTaraUnitConfezioni = -1, 0, riscontratiTaraUnitConfezioni)

                End If


                Dim newIdRegDettaglio As Integer = 0
                If movDetPrecedente IsNot Nothing AndAlso
                   movDetPrecedente.Movimenti_Dettagli_Tecnici_Extra IsNot Nothing AndAlso
                   movDetPrecedente.Movimenti_Dettagli_Tecnici_Extra.Count = 1 Then
                    newIdRegDettaglio = movDetPrecedente.Movimenti_Dettagli_Tecnici_Extra(0).Id_Reg_Dettaglio
                End If

                Dim objMovTecExtraD As New Movimento_Dettaglio_Tecnico_Extra With {
                    .Piva = objDettaglio.Piva,
                    .Sa_Cod = objDettaglio.Sa_Cod,
                    .Id_Agenda = objDettaglio.Id_Agenda,
                    .Id_Mov = objDettaglio.Id_Mov,
                    .Id_Mov_Det = objDettaglio.Id_Mov_Det,
                    .Id_Reg_Dettaglio = newIdRegDettaglio,
                    .Validita_Inizio = objDettaglio.Data,
                    .Unita_Trasporto = CInt(objContabDettaglio.ModuloGias),
                    .Num_Contenitori = If(objContabDettaglio.NumImballi, 0),
                    .Imballaggio_Cod = matCodImballo,
                    .Marche_Contenitori = If(dettaglioConfezionamento.FF_imballaggio_Descrizione, ""),
                    .Num_Colli = If(objContabDettaglio.NumContenitori, 0),
                    .Contenitore_Cod = matCodContenitore,
                    .Des_Contenitori = If(dettaglioConfezionamento.FF_contenitore_Descrizione, ""),
                    .Provvigione = objContabDettaglio.ProvvigionePercAgente,
                    .Provvigione_CapoArea = objContabDettaglio.ProvvigionePercCapoArea,
                    .Num_Conf_Riscontrate = riscontratiNumConfezioni,
                    .Num_Colli_Riscontrati = riscontratiNumContenitori,
                    .Num_Imballi_Riscontrati = riscontratiNumImballi,
                    .Tara_Unit_Conf_Riscontrata = riscontratiTaraUnitConfezioni,
                    .Tara_Unit_Collo_Riscontrata = riscontratiTaraUnitContenitori,
                    .Tara_Unit_Imballo_Riscontrata = riscontratiTaraUnitImballi,
                    .Peso_Netto_Riscontrato = If(objContabDettaglio.Riscontrati_PesoNetto, 0),
                    .Peso_Lordo_Riscontrato = If(objContabDettaglio.Riscontrati_PesoLordo, 0),
                    .N_Nota_DDT = objContabDettaglio.N_Nota_DDT,
                    .N_Nota_Riga_DDT = objContabDettaglio.N_Nota_Riga_DDT,
                    .Data_Nota_DDT = If(objContabDettaglio.Data_Nota_DDT, AGRODATAINIZIO),
                    .Username_Modifica = usernameOperatore
                }
                '.Peso_Lordo = ,

                Dim arrLavCodRifOrdine As Integer() = New Integer() {
                                                                        LAVCOD_ORDINE_VENDITA,
                                                                        LAVCOD_ORDINE_ACQUISTO,
                                                                        LAVCOD_BOLLA_EMESSA,
                                                                        LAVCOD_FATTURA_EMESSA,
                                                                        LAVCOD_DDT_CONTABILIZZATO_EMESSO,
                                                                        LAVCOD_BOLLA_RICEVUTA
                                                                    }

                If arrLavCodRifOrdine.Contains(lavCod) Then
                    objMovTecExtraD.N_Doc_Cliente = If(objContabDettaglio.N_Doc_Cliente, "")
                    objMovTecExtraD.Data_Doc_Cliente = If(objContabDettaglio.Data_Doc_Cliente, AGRODATAINIZIO)
                End If


                If movDetPrecedente IsNot Nothing AndAlso
                   movDetPrecedente.Movimenti_Dettagli_Tecnici_Extra IsNot Nothing AndAlso
                   movDetPrecedente.Movimenti_Dettagli_Tecnici_Extra.Count > 0 Then
                    objMovTecExtraD.Data_Creazione = movDetPrecedente.Movimenti_Dettagli_Tecnici_Extra(0).Data_Creazione
                    objMovTecExtraD.Username_Creazione = movDetPrecedente.Movimenti_Dettagli_Tecnici_Extra(0).Username_Creazione
                Else
                    objMovTecExtraD.Username_Creazione = usernameOperatore
                End If

                'Aggancio il movimento dettaglio tecnico extra sul movimento dettaglio
                objDettaglio.Movimenti_Dettagli_Tecnici_Extra = New List(Of Movimento_Dettaglio_Tecnico_Extra) From {
                    objMovTecExtraD
                }

            End If


            '------------------------------------------------
            '----- MOVIMENTO DETTAGLIO CONFERIMENTO
            '------------------------------------------------
            If UtilityHelper.IsAccettazione(lavCod) AndAlso
               objContabDettaglio.Conferimento_Speciale IsNot Nothing AndAlso
               objContabDettaglio.ElemCod <> RIGA_DESCRIZIONE_LIBERA Then

                'La fase_cod è in realtà memorizzata in movimenti_dettagli
                objDettaglio.Fase_Cod = If(objContabDettaglio.Conferimento_Speciale.FaseCodContratto, 0)

                Dim newIdRegDettaglio As Integer = 0
                If movDetPrecedente IsNot Nothing AndAlso
                   movDetPrecedente.Movimenti_Dettagli_Conferimento IsNot Nothing AndAlso
                   movDetPrecedente.Movimenti_Dettagli_Conferimento.Count = 1 Then
                    newIdRegDettaglio = movDetPrecedente.Movimenti_Dettagli_Conferimento(0).Id_Reg_Dettaglio
                End If

                Dim objMovConfD As New Movimento_Dettaglio_Conferimento With {
                    .Piva = objDettaglio.Piva,
                    .Sa_Cod = objDettaglio.Sa_Cod,
                    .Id_Agenda = objDettaglio.Id_Agenda,
                    .Id_Mov = objDettaglio.Id_Mov,
                    .Id_Mov_Det = objDettaglio.Id_Mov_Det,
                    .Id_Reg_Dettaglio = newIdRegDettaglio,
                    .Validita_Inizio = objDettaglio.Data,
                    .Tagliando_Pesa = If(objContabDettaglio.Conferimento_Speciale.TagliandoPesa, ""),
                    .Premio_Complessivo = If(objContabDettaglio.Conferimento_Speciale.PremioComplessivo, 0D),
                    .Cod_Varieta = If(objContabDettaglio.Conferimento_Speciale.CodVarieta, 0),
                    .Desc_Appezzamenti = If(objContabDettaglio.Conferimento_Speciale.DescAppezzamenti, ""),
                    .Username_Modifica = usernameOperatore
                }

                If movDetPrecedente IsNot Nothing AndAlso
                   movDetPrecedente.Movimenti_Dettagli_Conferimento IsNot Nothing AndAlso
                   movDetPrecedente.Movimenti_Dettagli_Conferimento.Count > 0 Then
                    objMovConfD.Data_Creazione = movDetPrecedente.Movimenti_Dettagli_Conferimento(0).Data_Creazione
                    objMovConfD.Username_Creazione = movDetPrecedente.Movimenti_Dettagli_Conferimento(0).Username_Creazione
                Else
                    objMovConfD.Username_Creazione = usernameOperatore
                End If

                'Aggancio il movimento dettaglio conferimento sul movimento dettaglio
                objDettaglio.Movimenti_Dettagli_Conferimento = New List(Of Movimento_Dettaglio_Conferimento) From {
                    objMovConfD
                }

            End If

            '------------------------------------------------
            '----- MOVIMENTO DETTAGLIO RIFERIMENTO
            '------------------------------------------------
            objDettaglio.Movimenti_Dettagli_Riferimenti = New List(Of Movimento_Dettaglio_Riferimento)

            If objContabDettaglio.ListRifMovDettaglio IsNot Nothing AndAlso
               objContabDettaglio.ListRifMovDettaglio.Count > 0 Then

                For Each contabRif In objContabDettaglio.ListRifMovDettaglio

                    If UtilityHelper.IsAccettazione(lavCod) AndAlso _lavCodScaricoRaccolta.Contains(contabRif.Lav_Cod_Rif) AndAlso contabRif.Piva_Rif = objContabDettaglio.PivaConferente Then
                        'Se è il collegamento tra conferimento e scarico da raccolta, lo devo ignorare perché la funzione ScriviScaricoRaccoltaAziendaCampagna se ne occupa a parte
                    ElseIf UtilityHelper.IsAccettazione(lavCod) AndAlso contabRif.Lav_Cod_Rif = LAVCOD_RACCOLTA Then
                        'Se è il collegamento tra conferimento e raccolta, lo devo ignorare perché la funzione ScriviRaccolteCollegate se ne occupa a parte 
                    Else

                        'Parte locale
                        'se l'ho letta da db avrò già valorizzato anche la prima parte, altrimenti non ce l'ho e la valorizzo come prima
                        'Prendo ad esempio la piva + id_agenda
                        Dim pivaLoc1parte As String = ""
                        Dim saCodLoc1parte As Integer = 0
                        Dim idAgendaLoc1parte As Integer = 0
                        Dim idMovLoc1parte As Integer = 0
                        Dim idMovDetLoc1parte As Integer = 0
                        Dim lavCodLoc1parte As Integer = 0
                        Dim cauMovLoc1parte As String = ""

                        Dim qtaLoc1parte As Decimal = 0

                        'TODO: Da sistemare perchè tanto è la scrittura del dettaglio che cambia di default la prima parte uguale a quella del dettaglio


                        'If Not String.IsNullOrEmpty(contabRif.Piva) AndAlso contabRif.Id_Agenda <> 0 Then
                        '    pivaLoc1parte = contabRif.Piva
                        '    saCodLoc1parte = contabRif.Sa_Cod
                        '    idAgendaLoc1parte = contabRif.Id_Agenda
                        '    idMovLoc1parte = contabRif.Id_Mov
                        '    idMovDetLoc1parte = contabRif.Id_Mov_Det
                        '    lavCodLoc1parte = contabRif.Lav_Cod
                        '    cauMovLoc1parte = contabRif.Cau_Mov

                        '    'In alcuni tipi di collegamento non mi interessa la qta,
                        '    'quindi se l'avevo letta come zero la rimetto a zero,
                        '    'altrimenti la aggiorno con quella di riga (che potrebbe essere cambiata perchè ho modificato la qta)
                        '    qtaLoc1parte = If(contabRif.Qta = 0, 0, objDettaglio.Qta)

                        'Else
                        pivaLoc1parte = objDettaglio.Piva
                        saCodLoc1parte = objDettaglio.Sa_Cod
                        idAgendaLoc1parte = objDettaglio.Id_Agenda
                        idMovLoc1parte = objDettaglio.Id_Mov
                        idMovDetLoc1parte = objDettaglio.Id_Mov_Det
                        lavCodLoc1parte = lavCod
                        cauMovLoc1parte = cauMov

                        qtaLoc1parte = objDettaglio.Qta

                        'End If


                        Dim objRiferimento As New Movimento_Dettaglio_Riferimento With {
                            .Piva = pivaLoc1parte,
                            .Sa_Cod = saCodLoc1parte,
                            .Id_Agenda = idAgendaLoc1parte,
                            .Id_Mov = idMovLoc1parte,
                            .Id_Mov_Det = idMovDetLoc1parte,
                            .Lav_Cod = lavCodLoc1parte,
                            .Cau_Mov = cauMovLoc1parte,
                            .Piva_Rif = contabRif.Piva_Rif,
                            .Sa_Cod_Rif = contabRif.Sa_Cod_Rif,
                            .Id_Agenda_Rif = contabRif.Id_Agenda_Rif,
                            .Id_Mov_Rif = contabRif.Id_Mov_Rif,
                            .Id_Mov_Det_Rif = contabRif.Id_Mov_Det_Rif,
                            .Lav_Cod_Rif = contabRif.Lav_Cod_Rif,
                            .Cau_Mov_Rif = contabRif.Cau_Mov_Rif,
                            .Qta = qtaLoc1parte,
                            .Preserva_Legame = contabRif.Preserva_Legame,
                            .Tipo_Associazione = contabRif.Tipo_Associazione,
                            .Username_Modifica = usernameOperatore
                        }

                        'TODO: come faccio a mantenere la data creazione se non so quale dei precedenti sto modificando?!?

                        If movDetPrecedente IsNot Nothing AndAlso
                            movDetPrecedente.Movimenti_Dettagli_Riferimenti IsNot Nothing AndAlso
                            movDetPrecedente.Movimenti_Dettagli_Riferimenti.Count > 0 Then
                            objRiferimento.Data_Creazione = movDetPrecedente.Movimenti_Dettagli_Riferimenti(0).Data_Creazione
                            objRiferimento.Username_Creazione = movDetPrecedente.Movimenti_Dettagli_Riferimenti(0).Username_Creazione
                        Else
                            objRiferimento.Username_Creazione = usernameOperatore
                        End If

                        objDettaglio.Movimenti_Dettagli_Riferimenti.Add(objRiferimento)

                    End If

                Next

            End If

            If objContabDettaglio.Tipo_Associazione = 1 AndAlso objContabDettaglio.Raccolte IsNot Nothing AndAlso objContabDettaglio.Raccolte.Count > 0 Then
                Dim movDetRifRaccolte As New List(Of Movimento_Dettaglio_Riferimento)

                For Each dettaglioRaccolta As Movimento_Dettaglio In objContabDettaglio.Raccolte

                    Dim dettaglioCaricoRaccolta As Movimento_Dettaglio

                    If dettaglioRaccolta.Cau_Mov = CAU_RILIEVO_RACCOLTA Then
                        'Modifico l'operazione di agenda di raccolta sull'azienda conferente creandone il movimento di carico per poterla collegare
                        dettaglioCaricoRaccolta = ScriviCaricoRaccoltaAziendaAgricola(dettaglioRaccolta, objContabDettaglio)
                    Else
                        dettaglioCaricoRaccolta = dettaglioRaccolta
                    End If

                    If dettaglioCaricoRaccolta.Id_Mov <> 0 Then
                        movDetRifRaccolte.Add(New Movimento_Dettaglio_Riferimento With {
                            .Piva = objDettaglio.Piva,
                            .Sa_Cod = objDettaglio.Sa_Cod,
                            .Id_Agenda = objDettaglio.Id_Agenda,
                            .Id_Mov = objDettaglio.Id_Mov,
                            .Id_Mov_Det = objDettaglio.Id_Mov_Det,
                            .Lav_Cod = lavCod,
                            .Cau_Mov = cauMov,
                            .Piva_Rif = objContabDettaglio.PivaConferente,
                            .Id_Agenda_Rif = dettaglioRaccolta.Id_Agenda,
                            .Sa_Cod_Rif = dettaglioCaricoRaccolta.Sa_Cod,
                            .Id_Mov_Rif = dettaglioCaricoRaccolta.Id_Mov,
                            .Id_Mov_Det_Rif = dettaglioCaricoRaccolta.Id_Mov_Det,
                            .Lav_Cod_Rif = LAVCOD_RACCOLTA,
                            .Cau_Mov_Rif = CAU_CARICO,
                            .Preserva_Legame = 0,
                            .Tipo_Associazione = 1
                        })
                    End If

                Next

                objDettaglio.Movimenti_Dettagli_Riferimenti.AddRange(movDetRifRaccolte)

            End If


            'Devo anche generare materie_prime_campionature; mi viene già passata la lista da fuori e la devo riempire con i default

            'If UtilityHelper.IsTrasformatoFF(objContabDettaglio.ModuloGias, objContabDettaglio.ElemCod) AndAlso
            '   ((objContabDettaglio.MateriePrimeCampionature IsNot Nothing AndAlso objContabDettaglio.MateriePrimeCampionature.Count > 0) OrElse
            '    (objContabDettaglio.Confezionamenti IsNot Nothing AndAlso objContabDettaglio.Confezionamenti.Count > 0)) Then

            '17/05/2021 Giulia: questa parte gliela faccio fare a prescindere che mi sia arrivato qualcosa o meno dal client,
            ' perché se ad esempio non ho scelto nessun parametro qualitativo ho bisogno che cmq venga assegnato un cal_cod (e ofornitore)
            If UtilityHelper.IsTrasformatoFF(objContabDettaglio.ModuloGias, objContabDettaglio.ElemCod) Then

                'Devo andare a scrivere "ofornitore" solo in carico
                If cauMov = CAU_CARICO Then
                    If objContabDettaglio.MateriePrimeCampionature Is Nothing Then
                        objContabDettaglio.MateriePrimeCampionature = New List(Of Materia_Prima_Campionatura)
                    End If

                    'lo devo aggiungere solo se già non c'era

                    If objContabDettaglio.MateriePrimeCampionature.FirstOrDefault(Function(x) x.Tipo = "ofornitore") Is Nothing Then
                        objContabDettaglio.MateriePrimeCampionature.Add(New Materia_Prima_Campionatura With {
                                                                               .Tipo = "ofornitore",
                                                                               .Tipo_Cod = objContabDettaglio.Fornitore
                                                                           }
                                                                        )
                    End If

                    'TODO: Per il momento lo lascio anche questo solo in carico ==> da rivedere
                    'Devo scrivere "onote" sempre, anche se il parametro non è abilitato
                    If objContabDettaglio.MateriePrimeCampionature.FirstOrDefault(Function(x) x.Tipo = "onote") Is Nothing Then
                        objContabDettaglio.MateriePrimeCampionature.Add(New Materia_Prima_Campionatura With {
                                                                           .Tipo = "onote",
                                                                           .Tipo_Cod = 0,
                                                                           .Val_Cod = 0,
                                                                           .Descrizione = objContabDettaglio.Note
                                                                           }
                                                                        )
                    End If

                End If


                Dim objMatPriCampHelper As New Agenda_Materie_Prime_Campionature_Helper

                Dim listMateriePrimeCamp As List(Of Materia_Prima_Campionatura)
                listMateriePrimeCamp = objMatPriCampHelper.RiempiListaConDefault(objContabDettaglio.Piva, objContabDettaglio.MateriePrimeCampionature,
                                                                                 _objParametriServer)


                'TODO: la parte dei confezionamenti sta su .Confezionamenti, quindi devo andarla a recuperare ed aggiornare le voci
                If objContabDettaglio.Confezionamenti IsNot Nothing AndAlso
                   objContabDettaglio.Confezionamenti.Count > 0 Then

                    If objContabDettaglio.Confezionamenti.Count > 1 Then
                        Throw New Exception(String.Format("Sono presenti {0} righe nella griglia dei confezionamenti",
                                                          objContabDettaglio.Confezionamenti.Count))
                    End If

                    'TODO: questi sono da fare solo se sono gestiti (Fruttagel ha solo imballaggi, quindi non dovrebbe inserire ance gli altri)!!!
                    Dim confezionamento As Contabilita_Riga_Confezionamento = objContabDettaglio.Confezionamenti(0)

                    InserisciConfezionamentoInCampionature(listMateriePrimeCamp, "oimballaggio",
                                                           confezionamento.FF_imballaggio_Tipo_Cod,
                                                           confezionamento.FF_imballaggio_Tara_Campionatura,
                                                           confezionamento.NrImballaggi)

                    InserisciConfezionamentoInCampionature(listMateriePrimeCamp, "ocontenitore",
                                                           confezionamento.FF_contenitore_Tipo_Cod,
                                                           confezionamento.FF_contenitore_Tara_Campionatura,
                                                           confezionamento.NrContenitori)

                    InserisciConfezionamentoInCampionature(listMateriePrimeCamp, "oconfezione",
                                                           confezionamento.FF_confezione_Tipo_Cod,
                                                           confezionamento.FF_confezione_Tara_Campionatura,
                                                           confezionamento.NrConfezioni)

                End If

                'Aggancio la lista sul movimento dettaglio
                If Not IsNothing(listMateriePrimeCamp) AndAlso listMateriePrimeCamp.Count > 0 Then

                    'Anche su tutte le Campionature devo impostare Validita_Inizio = Data del movimento che le crea
                    'Imposto anche il progressivo = cal_cod del dettaglio (così se era già valorizzato, questo permane)
                    For Each matCamp As Materia_Prima_Campionatura In listMateriePrimeCamp
                        matCamp.Progressivo = objContabDettaglio.CalCod

                        If cauMov = CAU_CARICO Then
                            matCamp.Validita_Inizio = objDettaglio.Data
                        End If
                    Next

                    objDettaglio.Materie_Prime_Campionature = listMateriePrimeCamp
                End If
            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return objDettaglio

    End Function

    Private Sub InserisciConfezionamentoInCampionature(ByRef listMateriePrimeCamp As List(Of Materia_Prima_Campionatura),
                                                       ByVal oTipo As String,
                                                       ByVal tipoCod As Integer?,
                                                       ByVal tara As Decimal?,
                                                       ByVal numeroElementi As Integer?)

        Const nomeRoutine = "ContabilitaHelper.InserisciConfezionamentoInCampionature()"

        Try

            'Se è Nothing vuol dire che non è gestito
            If tipoCod IsNot Nothing Then

                Dim item As Materia_Prima_Campionatura = listMateriePrimeCamp.Find(Function(x) x.Tipo = oTipo)

                If IsNothing(item) Then
                    Throw New Exception(String.Format("{0} presente in griglia confezionamenti, ma non gestito", oTipo))
                End If

                item.Tipo_Cod = tipoCod
                'TODO: Val_Cod?!?
                'item.Val_Cod = CStr(If(numeroElementi, 0))
                item.Val_Cod = 0
                item.ChkTara_Campionatura = 1
                item.Tara_Campionatura = If(tara, 0)

            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

    End Sub

    Private Function SpaccaRigheConfezionamentiDiversi(ByRef objContabDettaglio As Contabilita_Riga,
                                                       ByVal lavCod As Integer,
                                                       ByVal cauMov As String,
                                                       ByVal idMovT As Integer,
                                                       ByVal idMovCS As Integer,
                                                       ByVal dataOp As Date,
                                                       ByVal usernameOperatore As String,
                                                       ByVal messaggioDettagliato As Boolean,
                                                       ByRef msgError As String,
                                                       Optional ByVal objContabTestata As Contabilita_Testata = Nothing
                                                       ) As Contabilita_Output

        Const nomeRoutine = "ContabilitaHelper.SpaccaRigheConfezionamentiDiversi()"
        Dim objOutput As Contabilita_Output = Nothing

        Try

            'Devo mantenere tutto su una lista per coprire il fatto che potrei avere 2 righe come 10
            Dim numRighe As Integer = objContabDettaglio.Confezionamenti.Count

            'Devo conservare il peso totale perché devo far tornare tutti i conti
            Dim pesoNettoTotale As Decimal = objContabDettaglio.KgNetti
            Dim numImballiTotale As Decimal = objContabDettaglio.NumImballi
            Dim numContenitoriTotale As Decimal = objContabDettaglio.NumContenitori
            Dim numConfezioniTotale As Decimal = objContabDettaglio.NumConfezioni



            'TODO: copio objContabDettaglio in nuovo obj
            'TODO: nella fase di copia è meglio lasciare solo una riga in .Confezionamenti (ognuno la sua)
            'Key = indice di riga nell originale objContabDettaglio.Confezionamenti (se sono in modifica la Key=0 è la riga che c'era già su db --> update, tutte le altre insert)
            Dim dictContabDettagli As New SortedDictionary(Of Integer, Contabilita_Riga)

            For i As Integer = 1 To (numRighe - 1)
                Dim objTemp As Contabilita_Riga = objContabDettaglio.Clona(i)
                dictContabDettagli(i) = objTemp
            Next

            'Al termine della clonazione devo prendere il mio oggetto originario, lasciandogli solo la prima riga di confezionamento
            objContabDettaglio.Confezionamenti.RemoveRange(1, numRighe - 1)

            'Aggiorno i contatori della prima riga
            objContabDettaglio.NumImballi = If(objContabDettaglio.Confezionamenti(0).NrImballaggi, 0)
            objContabDettaglio.NumContenitori = If(objContabDettaglio.Confezionamenti(0).NrContenitori, 0)
            objContabDettaglio.NumConfezioni = If(objContabDettaglio.Confezionamenti(0).NrConfezioni, 0)

            'Inserisco anche la prima riga modificata nel dizionario
            dictContabDettagli(0) = objContabDettaglio


            'Prima di partire è necessario capire su quale livello devo proporzionare (il confezionamento presente più vicino al prodotto)
            Dim livelloRipartizione As enum_OTabelle
            Dim firstRowConf As Contabilita_Riga_Confezionamento = dictContabDettagli(0).Confezionamenti(0)

            If If(firstRowConf.NrConfezioni, 0) > 0 AndAlso If(firstRowConf.FF_confezione_Tipo_Cod, 0) <> 0 Then
                livelloRipartizione = enum_OTabelle.Confezione
            ElseIf If(firstRowConf.NrContenitori, 0) > 0 AndAlso If(firstRowConf.FF_contenitore_Tipo_Cod, 0) <> 0 Then
                livelloRipartizione = enum_OTabelle.Contenitore
            ElseIf If(firstRowConf.NrImballaggi, 0) > 0 AndAlso If(firstRowConf.FF_imballaggio_Tipo_Cod, 0) <> 0 Then
                livelloRipartizione = enum_OTabelle.Imballaggio
            Else
                Throw New Exception("Impossibile stabilire il livello sul quale riproporzionare il peso sui confezionamenti")
            End If




            'TODO: per contro-aggiornare: dopo aver salvato la riga lanciare query apposita che faccia tutte le somme direttamente su sql
            '(i valori sommati teoricamente me li dovrei anche restituire per poter aggiornare l'interfaccia della testata)

            'TODO: valori riscontrati per i num imballi in griglia, mentre i pesi riscontrati fuori griglia (responsabilità dell'utente di editarli)

            'TODO: mi devo cmq passare la chiave del movimento di testata (su cui faccio UPDATE) e la chiave dell'mov CS (devo sapere quali mov_dettagli devo analizzare)
            'TODO: recuperare tutti i valori sommatoria totale e vedere come li generava/salvava sull'xml il lan

            'Tot = 1000 Kg Pesca

            '3 Bins A --> 500 Kg    -->[1000/(3+2+1)]*3
            '2 Bins B --> 333.33 Kg -->[1000/(3+2+1)]*2
            '1 Bins C --> 166.66 Kg -->[1000/(3+2+1)]*1

            '3 Bins A --> 600 Kg    -->[1000/(3+1+1)]*3
            '1 Bins B --> 200 Kg    -->[1000/(3+1+1)]*1
            '1 Bins C --> 200 Kg    -->[1000/(3+1+1)]*1

            Dim sommatoriaPesi As Decimal = 0

            'TODO: leggo la configurazione per gli imballi

            Dim listDettagliImballo As New List(Of Dettaglio_Spaccamento)

            Dim hoTrovatolaConfigurazionePrecisaDiTutteLeRighe As Boolean = False
            Dim hoTrovatoUnaConfigurazioneParziale As Boolean = False
            Dim nonHoTrovatoConfigurazioni As Boolean = False



            'Leggo la tabella per il tipo di confezionamento scelto (ad es: 4 - Imballaggio)
            'Dalla riga di dettaglio so qual è il veg_cod ed il cul_cod del prodotto
            '==> leggo la tabella con Tabella_Cod = 4 e Veg_Cod = objContabDettaglio.VegCod e Cul_Cod IN (objContabDettaglio.CulCod, 0) [e ORDER BY CulCod DESC?!?]

            'Tengo in memoria la configurazione di tutti gli imballi per quel veg_cod (per evitare di fare molte letture)
            '   L'alternativa sarebbe quella di aggiungere Mat_Cod IN (...), ma poi cmq dovrei verificare se la riga esiste o no

            Dim dtConfigImballi As DataTable = Nothing
            If objContabDettaglio.VegCod IsNot Nothing AndAlso objContabDettaglio.VegCod <> 0 Then
                Dim filtroImballi As String = " (Cul_Cod IN (" & If(objContabDettaglio.CulCod, 0) & ", 0)) "
                Dim objConfigImballiR As New AgronicaCoreAnagrafeDAL.Configurazione_Imballaggi_R
                dtConfigImballi = objConfigImballiR.Leggi(0, objContabDettaglio.Piva, CInt(livelloRipartizione), 0,
                                                          objContabDettaglio.VegCod, 0,
                                                          filtroImballi, " Mat_Cod, Cul_Cod DESC ",
                                                          _objParametriServer)
            End If



            Dim numConfigurazioni As Integer = 0

            'Per ogni riga di nuovo dettaglio
            For Each item As KeyValuePair(Of Integer, Contabilita_Riga) In dictContabDettagli

                Dim rigaImballo As Contabilita_Riga_Confezionamento = item.Value.Confezionamenti(0)

                Dim numItemLivello As Decimal = 0
                Dim matCodLivello As Integer = 0

                Select Case livelloRipartizione

                    Case enum_OTabelle.Imballaggio
                        numItemLivello = item.Value.NumImballi
                        matCodLivello = UtilityHelper.GetMatCodImballo(rigaImballo, objContabDettaglio.Piva, _objParametriServer)

                    Case enum_OTabelle.Contenitore
                        numItemLivello = item.Value.NumContenitori
                        matCodLivello = UtilityHelper.GetMatCodContenitore(rigaImballo, objContabDettaglio.Piva, _objParametriServer)

                    Case enum_OTabelle.Confezione
                        numItemLivello = item.Value.NumConfezioni
                        matCodLivello = UtilityHelper.GetMatCodConfezione(rigaImballo)

                End Select


                'Verifico se ho mat_cod imballo con la varietà precisa
                'Altrimenti verifico se ho imballo con Cul_Cod = 0 (forse mi basterebbe avere le righe in ordine [da verificare se facendo select si mantiene ORDER (potrei cmq ordinare qui in questa select)])

                Dim objConfig As Object = Nothing
                If dtConfigImballi IsNot Nothing AndAlso dtConfigImballi.Rows.Count > 0 Then
                    objConfig = (From dr In dtConfigImballi.Rows
                                 Where CInt(dr("Mat_Cod")) = matCodLivello
                                 Select dr
                                 Order By dr("Cul_Cod") Descending).FirstOrDefault()
                End If

                If objConfig IsNot Nothing Then

                    'TODO: teoricamente potrei aver spezzato su più righe con lo stesso mat_cod ==> meglio usare un dizionario? (cmq verificare che non abbia già inserito la riga per quel mat_cod)

                    'Avendo ordinato, prendo sempre la rima riga (che quindi sarà quella col Cul_Cod preciso, se esiste, altrimenti Cul_Cod 0)
                    listDettagliImballo.Add(New Dettaglio_Spaccamento With {
                                               .Nr_Riga = item.Key,
                                               .Mat_Cod = matCodLivello,
                                               .Capacita = CDec(objConfig.Item("Valore")),
                                               .Quantita = numItemLivello
                                            })
                    numConfigurazioni += 1

                Else

                    'Non c'è configurazione per quell'imballo
                    listDettagliImballo.Add(New Dettaglio_Spaccamento With {
                                               .Nr_Riga = item.Key,
                                               .Mat_Cod = matCodLivello,
                                               .Capacita = 0,
                                               .Quantita = numItemLivello
                                            })

                End If

            Next


            'Devo capire se ho configurazione totale / Parziale / Nessuna
            If numConfigurazioni = 0 Then
                nonHoTrovatoConfigurazioni = True
            ElseIf numConfigurazioni = dictContabDettagli.Count Then
                hoTrovatolaConfigurazionePrecisaDiTutteLeRighe = True
            Else
                'Il caso di configurazione parziale lo trattiamo come se non ci fosse alcuna configurazione
                'hoTrovatoUnaConfigurazioneParziale = True
                nonHoTrovatoConfigurazioni = True
            End If



            'TODO: richiamo l'algoritmo che mi dice come devo dividere la qta
            If hoTrovatolaConfigurazionePrecisaDiTutteLeRighe Then

                'Devo calcolare la capacità totale di tutti gli imballi
                Dim capacitaTotale As Decimal = listDettagliImballo.Sum(Function(x) x.Capacita)


                For Each item As KeyValuePair(Of Integer, Contabilita_Riga) In dictContabDettagli

                    'Recupero la riga di configurazione corretta
                    Dim objSpacca As Dettaglio_Spaccamento = listDettagliImballo.FirstOrDefault(Function(x) x.Nr_Riga = item.Key)

                    item.Value.KgNetti = ArrotondaVal_3((pesoNettoTotale / capacitaTotale) * objSpacca.Capacita)

                    Dim firstRowConfItem As Contabilita_Riga_Confezionamento = item.Value.Confezionamenti(0)
                    item.Value.Tara = (CDec(If(firstRowConfItem.NrConfezioni, 0)) * CDec(If(firstRowConfItem.FF_confezione_Tara_Campionatura, 0))) +
                                      (CDec(If(firstRowConfItem.NrContenitori, 0)) * CDec(If(firstRowConfItem.FF_contenitore_Tara_Campionatura, 0))) +
                                      (CDec(If(firstRowConfItem.NrImballaggi, 0)) * CDec(If(firstRowConfItem.FF_imballaggio_Tara_Campionatura, 0)))
                    item.Value.KgLordi = item.Value.KgNetti + item.Value.Tara
                    sommatoriaPesi += item.Value.KgNetti


                    'TODO: se avevo gli impianti per la raccolta, devo riproporzionare la loro qta nello stesso modo dei kg netti di riga
                    If Not IsNothing(item.Value.RigheImpianti) AndAlso item.Value.RigheImpianti.Count > 0 Then

                        Dim sommatoriaQtaImpianti As Decimal = 0

                        For Each rowImpianto In item.Value.RigheImpianti
                            Dim qtaAttualeImpianto As Decimal = rowImpianto.Qta
                            'TODO: dovrei fare la stessa cosa che faccio per i kg netti, quindi (qtaAttualeImpianto / capacitaTotale) * objSpacca.Capacita
                            Dim newQtaImpianto As Decimal = ArrotondaVal_2((qtaAttualeImpianto / capacitaTotale) * objSpacca.Capacita)
                            sommatoriaQtaImpianti += newQtaImpianto
                            rowImpianto.Qta = newQtaImpianto
                        Next

                        'Devo aggiustare le qta in maniera che la loro somma sia uguale a quella iniziale
                        'TODO: Devo ritoccare (il primo? l'ultimo? il più grande?), ora è il primo così ce l'ho sempre
                        Dim diffQtaImpianti As Decimal = item.Value.KgNetti - sommatoriaQtaImpianti

                        If diffQtaImpianti <> 0 Then
                            item.Value.RigheImpianti(0).Qta += diffQtaImpianti
                        End If

                    End If

                Next


                'Throw New NotImplementedException("Implementare algoritmo quando tutte le righe coinvolte hanno la configurazione")

            ElseIf hoTrovatoUnaConfigurazioneParziale Then

                Throw New NotImplementedException("Implementare algoritmo quando solo alcune delle righe coinvolte hanno la configurazione")

            Else

                'Nessuna riga ha la configurazione ==> Verranno create 2+ righe di movimento facendo proporzioni sui numeri imballi

                For Each item As KeyValuePair(Of Integer, Contabilita_Riga) In dictContabDettagli

                    Dim numTotaleLivello As Decimal = 0
                    Select Case livelloRipartizione
                        Case enum_OTabelle.Imballaggio
                            numTotaleLivello = numImballiTotale
                        Case enum_OTabelle.Contenitore
                            numTotaleLivello = numContenitoriTotale
                        Case enum_OTabelle.Confezione
                            numTotaleLivello = numConfezioniTotale
                    End Select

                    'Recupero la riga di configurazione corretta
                    Dim objSpacca As Dettaglio_Spaccamento = listDettagliImballo.FirstOrDefault(Function(x) x.Nr_Riga = item.Key)

                    item.Value.KgNetti = ArrotondaVal_3((pesoNettoTotale / numTotaleLivello) * objSpacca.Quantita)

                    Dim firstRowConfItem As Contabilita_Riga_Confezionamento = item.Value.Confezionamenti(0)
                    item.Value.Tara = (CDec(If(firstRowConfItem.NrConfezioni, 0)) * CDec(If(firstRowConfItem.FF_confezione_Tara_Campionatura, 0))) +
                                      (CDec(If(firstRowConfItem.NrContenitori, 0)) * CDec(If(firstRowConfItem.FF_contenitore_Tara_Campionatura, 0))) +
                                      (CDec(If(firstRowConfItem.NrImballaggi, 0)) * CDec(If(firstRowConfItem.FF_imballaggio_Tara_Campionatura, 0)))
                    item.Value.KgLordi = item.Value.KgNetti + item.Value.Tara
                    sommatoriaPesi += item.Value.KgNetti


                    'TODO: se avevo gli impianti per la raccolta, devo riproporzionare la loro qta nello stesso modo dei kg netti di riga
                    If Not IsNothing(item.Value.RigheImpianti) AndAlso item.Value.RigheImpianti.Count > 0 Then

                        Dim sommatoriaQtaImpianti As Decimal = 0

                        For Each rowImpianto In item.Value.RigheImpianti
                            Dim qtaAttualeImpianto As Decimal = rowImpianto.Qta
                            Dim newQtaImpianto As Decimal = ArrotondaVal_2((qtaAttualeImpianto / numTotaleLivello) * objSpacca.Quantita)
                            sommatoriaQtaImpianti += newQtaImpianto
                            rowImpianto.Qta = newQtaImpianto
                        Next

                        'Devo aggiustare le qta in maniera che la loro somma sia uguale a quella iniziale
                        'TODO: Devo ritoccare (il primo? l'ultimo? il più grande?), ora è il primo così ce l'ho sempre
                        Dim diffQtaImpianti As Decimal = item.Value.KgNetti - sommatoriaQtaImpianti

                        If diffQtaImpianti <> 0 Then
                            item.Value.RigheImpianti(0).Qta += diffQtaImpianti
                        End If

                    End If

                Next

            End If





            'TODO: Arrotonda_Val 3 ?!?



            'Devo aggiustare i pesi in maniera che la loro somma sia uguale a quella iniziale
            'TODO: Devo ritoccare (il primo? l'ultimo? il più grande?), ora è il primo così ce l'ho sempre

            Dim diffPesi As Decimal = pesoNettoTotale - sommatoriaPesi

            If diffPesi <> 0 Then

                dictContabDettagli(0).KgNetti += diffPesi

                'TODO: se cambio i kg netti potrei dover ritoccare anche le qta degli impianti
                If dictContabDettagli(0).RigheImpianti IsNot Nothing AndAlso dictContabDettagli(0).RigheImpianti.Count > 0 Then

                    dictContabDettagli(0).RigheImpianti(0).Qta += diffPesi

                End If

            End If


            'Avendo cambiato la qta, devo richiamare la funzione che aggiorna i calcoli economici su tutti gli obj
            For Each item As KeyValuePair(Of Integer, Contabilita_Riga) In dictContabDettagli
                objOutput = New Contabilita_Output()

                UtilityHelper.AggiornaDettagliEconomici(item.Value, lavCod)

                'TODO: Transazione?!? Teoricamente l'ha già aperta il chiamante, quindi dovrebbe essere a posto così

                'TODO: devo far partire la procedura di modifica sulla prima riga/obj, mentre la seconda deve essere in insert!
                If item.Value.IdMovDet <> 0 Then

                    'Si tratta della riga originaria ==> UPDATE!!!
                    objOutput = AggiornaSingolaRigaDocumento(item.Value,
                                                             lavCod, cauMov,
                                                             idMovT, idMovCS,
                                                             dataOp, usernameOperatore,
                                                             messaggioDettagliato, objContabTestata)
                    If objOutput Is Nothing OrElse objOutput.Risultato = False Then
                        Throw New Exception(String.Format("Errore Aggiornamento riga {0}: {1}",
                                                          item.Key, If(objOutput.MsgError, "")))
                    End If

                Else

                    'Si tratta di una delle nuove righe generate ==> INSERT!!!
                    objOutput = ScriviSingolaRigaDocumento(item.Value,
                                                           lavCod, cauMov,
                                                           idMovT, idMovCS,
                                                           dataOp, usernameOperatore,
                                                           messaggioDettagliato, objContabTestata)
                    If objOutput Is Nothing OrElse objOutput.Risultato = False Then
                        Throw New Exception(String.Format("Errore Inserimento riga {0}: {1}",
                                                          item.Key, If(objOutput.MsgError, "")))
                    End If

                End If

            Next

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        'Di fatto viene ritornato l'output dell'ultima scrittura/aggiornamento
        '(quello che mi interessa da fuori è il conteggio totali)
        'Se fosse stata la prima riga in assoluto del documento, la testata l'ho già scritta prima di arrivare a questa funzione,
        ' quindi la parte di output riguardante la testata la ottengo da fuori e poi mergerò i due output
        Return objOutput

    End Function

    Private Class Dettaglio_Spaccamento
        Public Property Nr_Riga As Integer
        Public Property Mat_Cod As Integer
        Public Property Capacita As Decimal
        Public Property Quantita As Decimal
    End Class

    Public Shared Sub SistemaValoriRigaPerScrittura(ByRef objContabDettaglio As Contabilita_Riga,
                                                    ByVal lavCod As Integer)

        Const nomeRoutine = "ContabilitaHelper.SistemaValoriRigaPerScrittura()"
        Dim objContabHelper As New AgronicaCoreContabHLP.Contabilita
        Dim kgDegrado As Decimal
        Dim kgNettiPesoPagamento As Decimal

        Try

            objContabDettaglio.Degrado = If(objContabDettaglio.Degrado, 0)
            If objContabDettaglio.Degrado <> 0 Then
                'TODO: è da lasciare arrotondato all'intero?
                kgDegrado = ArrotondaVal_0(objContabDettaglio.KgNetti / 100 * objContabDettaglio.Degrado)
            Else
                kgDegrado = 0D
            End If

            kgNettiPesoPagamento = objContabDettaglio.KgNetti - kgDegrado

            'TODO: cose da aggiornare:

            'TODO: in FF per 210: udm e qta possono essere 38 o 2 a seconda se c'è o meno il livello confezioni

            '=====================================================================================================
            'in caso di gestione delle udm custom --> riporto l'udm a numero o kg (dipende se è impostata la confezione)
            '-----------------------------------------------------------------------------------------------------
            If UtilityHelper.IsTrasformatoFF(objContabDettaglio.ModuloGias, objContabDettaglio.ElemCod) = False Then

                'Standard
                objContabDettaglio.DB_Qta = objContabDettaglio.Quantita
                objContabDettaglio.DB_Udm_Cod = objContabDettaglio.UdM
                objContabDettaglio.DB_Prezzo_Unitario = objContabDettaglio.Prezzo
                objContabDettaglio.DB_Prezzo_Unitario_Netto = objContabDettaglio.PrezzoNetto
                objContabDettaglio.DB_Udm_Cod_Extra = objContabDettaglio.Udm_Cod_Extra
                objContabDettaglio.DB_Qta_Extra = objContabDettaglio.Qta_Extra

            Else

                If objContabDettaglio.UdM = enum_UnitaMisura.KG Then

                    Select Case Modalita_Imballaggio_FF(objContabDettaglio)

                        Case enum_OTabelle.Nessuno

                            'Peso Sfuso
                            objContabDettaglio.DB_Udm_Cod = enum_UnitaMisura.KG
                            objContabDettaglio.DB_Udm_Cod_Extra = 0

                            objContabDettaglio.DB_Qta = objContabDettaglio.KgNetti
                            objContabDettaglio.DB_Qta_Extra = 1

                            objContabDettaglio.DB_Prezzo_Unitario = objContabDettaglio.PrezzoEffettivoKgL

                            If kgNettiPesoPagamento <> 0 Then
                                objContabDettaglio.DB_Prezzo_Unitario_Netto = ArrotondaVal_6(objContabDettaglio.ImponibileTotaleNetto / kgNettiPesoPagamento)
                            Else
                                objContabDettaglio.DB_Prezzo_Unitario_Netto = 0
                            End If


                        Case enum_OTabelle.Contenitore, enum_OTabelle.Imballaggio

                            objContabDettaglio.DB_Udm_Cod = enum_UnitaMisura.KG
                            objContabDettaglio.DB_Udm_Cod_Extra = enum_UnitaMisura.KG 'Tengo l'unità di misura in kg in modo da distinguerlo dal peso sfuso.

                            objContabDettaglio.DB_Qta = objContabDettaglio.KgNetti
                            objContabDettaglio.DB_Qta_Extra = 1

                            objContabDettaglio.DB_Prezzo_Unitario = objContabDettaglio.PrezzoEffettivoKgL

                            If kgNettiPesoPagamento <> 0 Then
                                objContabDettaglio.DB_Prezzo_Unitario_Netto = ArrotondaVal_6(objContabDettaglio.ImponibileTotaleNetto / kgNettiPesoPagamento)
                            Else
                                objContabDettaglio.DB_Prezzo_Unitario_Netto = 0
                            End If

                        Case enum_OTabelle.Confezione

                            'Confezione Impostata --> Numero
                            objContabDettaglio.DB_Udm_Cod = enum_UnitaMisura.Numero
                            objContabDettaglio.DB_Udm_Cod_Extra = enum_UnitaMisura.KG

                            objContabDettaglio.DB_Qta = objContabDettaglio.NumConfezioni
                            objContabDettaglio.DB_Qta_Extra = ArrotondaVal_6(objContabDettaglio.KgNetti / objContabDettaglio.NumConfezioni)

                            objContabDettaglio.DB_Prezzo_Unitario = objContabDettaglio.Prezzo
                            objContabDettaglio.DB_Prezzo_Unitario_Netto = objContabDettaglio.PrezzoNetto

                        Case Else

                            Throw New Exception("Gestione Qta Imballaggi errata.")

                    End Select

                Else
                    objContabDettaglio.DB_Qta = objContabDettaglio.Quantita
                    objContabDettaglio.DB_Udm_Cod = objContabDettaglio.UdM
                    objContabDettaglio.DB_Prezzo_Unitario = objContabDettaglio.Prezzo
                    objContabDettaglio.DB_Prezzo_Unitario_Netto = objContabDettaglio.PrezzoNetto

                    objContabDettaglio.DB_Udm_Cod_Extra = objContabDettaglio.Udm_Cod_Extra
                    objContabDettaglio.DB_Qta_Extra = objContabDettaglio.Qta_Extra

                End If

                'Controllo correzione livello
                If (objContabDettaglio.PrezzoRiferitoA <> enum_PrezzoLivello.Udm_principale) AndAlso
                       (objContabDettaglio.PrezzoRiferitoA <> enum_PrezzoLivello.Confezione OrElse
                        objContabDettaglio.PrezzoRiferitoA = enum_PrezzoLivello.Kg_Litri) Then

                    If objContabDettaglio.PrezzoRiferitoA = enum_PrezzoLivello.Kg_Litri Then
                        objContabDettaglio.DB_Prezzo_Unitario = objContabDettaglio.PrezzoEffettivoKgL

                        If kgNettiPesoPagamento <> 0 Then
                            objContabDettaglio.DB_Prezzo_Unitario_Netto = ArrotondaVal_6(objContabDettaglio.ImponibileTotaleNetto / kgNettiPesoPagamento)
                        Else
                            objContabDettaglio.DB_Prezzo_Unitario_Netto = 0
                        End If
                    Else
                        objContabDettaglio.DB_Prezzo_Unitario = objContabDettaglio.Prezzo
                        objContabDettaglio.DB_Prezzo_Unitario_Netto = objContabDettaglio.PrezzoNetto
                    End If

                End If

            End If

            'Segno Imponibile e Imponibile_Netto
            objContabDettaglio.ImponibileTotale = objContabHelper.Leggi_Imponibile_PositivoNegativo(lavCod, objContabDettaglio.ImponibileTotale)
            objContabDettaglio.ImponibileTotaleNetto = objContabHelper.Leggi_Imponibile_PositivoNegativo(lavCod, objContabDettaglio.ImponibileTotaleNetto)

            'Segno IVA
            objContabDettaglio.Iva = objContabHelper.Leggi_IVA_PositivaNegativa(lavCod, objContabDettaglio.Iva)

            'Segno IVA Indetraibile
            objContabDettaglio.IvaIndetraibile = objContabHelper.Leggi_IVAIndet_PositivaNegativa(lavCod, objContabDettaglio.IvaIndetraibile)

            If Not UtilityHelper.IsMovimentoMagazzino(lavCod) Then

                'Conti Economici e Patrimoniali di default

                If objContabDettaglio.ContoEconomico Is Nothing OrElse objContabDettaglio.ContoEconomico = 0 Then
                    'Setto il default in base al tipo di documento
                    objContabDettaglio.ContoEconomico = UtilityHelper.GetConto(lavCod, enumTipoConto.Economico, 0)
                End If

                If objContabDettaglio.ContoPatrimoniale Is Nothing OrElse objContabDettaglio.ContoPatrimoniale = 0 Then
                    'Setto il default in base al tipo di documento
                    objContabDettaglio.ContoPatrimoniale = UtilityHelper.GetConto(lavCod, enumTipoConto.Patrimoniale, 0)
                End If

            End If

            'TODO: segno sconto

            'TODO: peso e tipo_peso?!?

            'Stringa testo e percentuale totale degli sconti addizionali a cascata
            UtilityHelper.CalcolaScontoAddizionaleCascata(objContabDettaglio)

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message & If(ex.InnerException Is Nothing, "", " [" & ex.InnerException.Message & "]"))
        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objContabDettaglio"></param>
    ''' <param name="lavCod"></param>
    Private Sub AggiustaUdmPerScrittura(ByRef objContabDettaglio As Contabilita_Riga, ByVal lavCod As Integer)

        Const nomeRoutine = "ContabilitaHelper.AggiustaUdmPerScrittura()"

        Dim isTrasformatoFF As Boolean = UtilityHelper.IsTrasformatoFF(objContabDettaglio.ModuloGias, objContabDettaglio.ElemCod)
        Dim listaUdmConverti As New List(Of Integer) From {enum_UnitaMisura.Quintali, enum_UnitaMisura.Tonnellate, enum_UnitaMisura.Metri_Cubi}
        Dim listaPrezzoRifConverti As New List(Of Integer) From {enum_PrezzoLivello.Udm_principale, enum_PrezzoLivello.Kg_Litri}
        Dim listaValoreRifPrezzoConverti As New List(Of Integer) From {enum_EditImporto.PrezzoUnitario, enum_EditImporto.Importo_Unitario}

        Try
            'qta, kgnetti, lordi, udm, extra_int, campo del prezzo imputato dall'utente in base a ValoreRiferimentoPrezzo e PrezzoRiferitoA
            If listaUdmConverti.Contains(objContabDettaglio.UdM) Then

                objContabDettaglio.ExtraInt = objContabDettaglio.UdM

                If objContabDettaglio.UdM = enum_UnitaMisura.Metri_Cubi Then
                    objContabDettaglio.UdM = enum_UnitaMisura.Litri
                Else
                    objContabDettaglio.UdM = enum_UnitaMisura.KG
                End If

                Dim qtaIng, qtaUsc, prezzoIng, prezzoUsc As Decimal?
                Dim qtaIngAgg(), qtaUscAgg(), przIngAgg(), przUscAgg() As Decimal

                qtaIng = objContabDettaglio.Quantita
                qtaIngAgg = {objContabDettaglio.KgNetti, objContabDettaglio.KgLordi}

                'Individuo se devo convertire uno dei campi del prezzo:
                If listaPrezzoRifConverti.Contains(objContabDettaglio.PrezzoRiferitoA) AndAlso
                    listaValoreRifPrezzoConverti.Contains(objContabDettaglio.ValoreRiferimentoPrezzo) Then

                    prezzoIng = objContabDettaglio.Prezzo
                    przIngAgg = {objContabDettaglio.ImportoUnitario, objContabDettaglio.PrezzoEffettivoKgL}

                End If

                UnitaMisura_R.ConvertiQtaPrezzi(objContabDettaglio.ExtraInt, objContabDettaglio.UdM, qtaIng, qtaUsc, prezzoIng, prezzoUsc,
                                                qtaIngAgg, qtaUscAgg, przIngAgg, przUscAgg)

                objContabDettaglio.Quantita = qtaUsc
                objContabDettaglio.KgNetti = qtaUscAgg(0)
                objContabDettaglio.KgLordi = qtaUscAgg(1)

                If listaPrezzoRifConverti.Contains(objContabDettaglio.PrezzoRiferitoA) AndAlso
                    listaValoreRifPrezzoConverti.Contains(objContabDettaglio.ValoreRiferimentoPrezzo) Then

                    objContabDettaglio.Prezzo = prezzoUsc
                    objContabDettaglio.ImportoUnitario = przUscAgg(0)
                    objContabDettaglio.PrezzoEffettivoKgL = przUscAgg(1)

                End If

                'If listaPrezzoRifConverti.Contains(objContabDettaglio.PrezzoRiferitoA) Then
                '    Select Case objContabDettaglio.ValoreRiferimentoPrezzo

                '        Case enum_EditImporto.PrezzoUnitario
                '            prezzoIng = objContabDettaglio.Prezzo

                '        Case enum_EditImporto.Importo_Unitario
                '            prezzoIng = objContabDettaglio.ImportoUnitario

                '        Case Else
                '            'Negli altri casi non devo fare conversioni, quindi lascio prezzoIng a nothing
                '    End Select
                'End If

                'If isTrasformatoFF Then
                '    qtaIng = objContabDettaglio.KgNetti
                '    qtaIngAgg = {objContabDettaglio.KgLordi}

                '    UnitaMisura_R.ConvertiQtaPrezzi(objContabDettaglio.ExtraInt, objContabDettaglio.UdM, qtaIng, qtaUsc, prezzoIng, prezzoUsc, qtaIngAgg, qtaUscAgg)

                '    objContabDettaglio.KgNetti = qtaUsc
                '    objContabDettaglio.KgLordi = qtaUscAgg(0)
                'Else
                '    qtaIng = objContabDettaglio.Quantita

                '    UnitaMisura_R.ConvertiQtaPrezzi(objContabDettaglio.ExtraInt, objContabDettaglio.UdM, qtaIng, qtaUsc, prezzoIng, prezzoUsc)

                '    objContabDettaglio.Quantita = qtaUsc
                'End If

                ''Dopo l'eventuale conversione, riassegno al giusto campo il valore ottenuto
                'If listaPrezzoRifConverti.Contains(objContabDettaglio.PrezzoRiferitoA) Then
                '    Select Case objContabDettaglio.ValoreRiferimentoPrezzo

                '        Case enum_EditImporto.PrezzoUnitario
                '            objContabDettaglio.Prezzo = prezzoUsc
                '            objContabDettaglio.PrezzoEffettivoKgL = prezzoUsc

                '        Case enum_EditImporto.Importo_Unitario
                '            objContabDettaglio.ImportoUnitario = prezzoUsc
                '    End Select
                'End If

            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message & If(ex.InnerException Is Nothing, "", " [" & ex.InnerException.Message & "]"))
        End Try

    End Sub

    Private Shared Function Modalita_Imballaggio_FF(ByVal objContabDettaglio As Contabilita_Riga
                                                    ) As enum_OTabelle

        Dim modalitaConfezionamento As enum_OTabelle = enum_OTabelle.Nessuno

        If UtilityHelper.IsTrasformatoFF(objContabDettaglio.ModuloGias, objContabDettaglio.ElemCod) = True Then


            If objContabDettaglio.NumImballi <> 0 Then
                modalitaConfezionamento = enum_OTabelle.Imballaggio
            End If

            If objContabDettaglio.NumContenitori <> 0 Then
                modalitaConfezionamento = enum_OTabelle.Contenitore
            End If

            If objContabDettaglio.NumConfezioni <> 0 Then
                modalitaConfezionamento = enum_OTabelle.Confezione
            End If

        End If

        Return modalitaConfezionamento

    End Function

#Region "AggiornaDettagliEconomiciDaPrezzo"

    Public Shared Function AggiornaDettagliEconomiciDaPrezzoKg(ByVal piva As String,
                                                               ByVal id_Mov_Det As Integer,
                                                               ByVal prezzo_Unitario As Decimal,
                                                               ByVal aggiornaSoloSeZero As Boolean,
                                                               ByVal prezzoDaRigaConferim As Integer,
                                                               ByRef objParametri As AgronicaCoreParametri,
                                                               Optional ByRef dal As Gias_DeveloperServer_Entities = Nothing,
                                                               Optional ByVal cod_iva As Integer? = Nothing,
                                                               Optional ByVal moduloGias As Integer = -1,
                                                               Optional ByRef listaIva As List(Of IVA_Aliquote) = Nothing,
                                                               Optional ByVal bValorizzazioneoConferimenti As Boolean = False,
                                                               Optional ByVal riaperturaLiquidazione As Boolean = False
                                                               ) As String

        Dim risultato As String = String.Empty
        Dim pivaSuperUser As String = objParametri.PivaSuperUser

        If dal Is Nothing Then
            Dim gefutils As New Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            dal = New Gias_DeveloperServer_Entities(efConnString)
        End If

        Try

            ' Leggo riga di movimento dettaglio
            Dim rigaMovDet = (From md In dal.Movimenti_dettagli
                              Join a In dal.Agenda
                              On md.PIVA Equals a.PIVA And md.Id_Agenda Equals a.Id_Agenda
                              Where md.PIVA = piva AndAlso
                                    md.Id_Mov_Det = id_Mov_Det
                              Select a.Lav_Cod, md).FirstOrDefault()

            If rigaMovDet Is Nothing Then
                Return String.Format("Non è stata trovata la riga con chiave {0} ", id_Mov_Det)
            End If

            If Not bValorizzazioneoConferimenti AndAlso
                Not riaperturaLiquidazione Then
                If (prezzoDaRigaConferim = 1 AndAlso CDec(rigaMovDet.md.Prezzo_Unitario) = 0D) OrElse
               (prezzoDaRigaConferim = 0 AndAlso CDec(rigaMovDet.md.Prezzo_Unitario) <> 0D) Then
                    Return "Rilevata discordanza sulla valorizzazione dei prezzi tra conferimento e liquidazione. Rieseguire il calcolo."
                End If
            Else
                'Il prezzo deve essere sempre sovrascritto
            End If

            If aggiornaSoloSeZero = False OrElse
               (aggiornaSoloSeZero = True AndAlso CDec(rigaMovDet.md.Prezzo_Unitario) = 0D) Then

                ' Leggo Modulo Generazione
                If moduloGias = -1 Then
                    Dim oGenerazione = (From o In dal.OGenerazioni_Anagrafe_Moduli_Log
                                        Where o.Piva_SuperUser = pivaSuperUser AndAlso
                                              o.Piva = piva
                                        Select o).FirstOrDefault()
                    moduloGias = If(oGenerazione Is Nothing, 0, oGenerazione.Modulo_Generazione)
                End If

                'Iva / Aliquota iva
                Dim aliquota_Iva As Decimal? = 0
                If cod_iva Is Nothing Then
                    ' Leggo Aliquota Iva da movimenti dettagli
                    If rigaMovDet.md.Cod_Iva.HasValue AndAlso rigaMovDet.md.Cod_Iva <> 0 AndAlso rigaMovDet.md.Cod_Iva <> -1 Then
                        cod_iva = rigaMovDet.md.Cod_Iva
                    Else
                        cod_iva = 0
                    End If
                End If

                If listaIva IsNot Nothing AndAlso listaIva.Count > 0 Then
                    aliquota_Iva = (From iv In listaIva Where iv.Codice = CInt(cod_iva) Select iv.Aliquota).FirstOrDefault()
                Else
                    aliquota_Iva = (From iv In dal.IVA_Aliquote Where iv.Codice = CInt(cod_iva) Select iv.Aliquota).FirstOrDefault()
                End If

                ' Imposto qta / Numero Confezioni
                Dim qta As Decimal? = Nothing
                Dim numero_confezioni As Decimal? = Nothing
                If UtilityHelper.IsTrasformatoFF(moduloGias, rigaMovDet.md.Elem_Cod) = False Then
                    qta = rigaMovDet.md.Qta
                Else
                    If rigaMovDet.md.Udm_Cod = enum_UnitaMisura.Numero Then
                        numero_confezioni = rigaMovDet.md.Qta
                    Else
                        numero_confezioni = 0
                    End If
                End If

                ' Ricavo sconti Addizionali
                Dim scontoAddiz1 As Decimal? = Nothing
                Dim scontoAddiz2 As Decimal? = Nothing
                Dim scontoAddiz3 As Decimal? = Nothing
                Dim valoreDecimale As Decimal = 0
                If Not String.IsNullOrEmpty(rigaMovDet.md.Sconto_Testo) Then

                    Dim arraySconti As String() = rigaMovDet.md.Sconto_Testo.Split("-")
                    If Decimal.TryParse(arraySconti(0), valoreDecimale) Then
                        scontoAddiz1 = valoreDecimale
                    End If

                    If arraySconti.Length > 1 AndAlso Decimal.TryParse(arraySconti(1), valoreDecimale) Then
                        scontoAddiz2 = valoreDecimale
                    End If

                    If arraySconti.Length > 2 AndAlso Decimal.TryParse(arraySconti(2), valoreDecimale) Then
                        scontoAddiz3 = valoreDecimale
                    End If

                End If
                Dim scontoCalcolato As Decimal = ArrotondaVal_6(AgronicaCoreContabHLP.Contabilita.CalcolaScontoComplessivo(
                    Math.Abs(CDec(rigaMovDet.md.Sconto)),
                    scontoAddiz1,
                    scontoAddiz2,
                    scontoAddiz3))

                Dim contabilita_riga As New Contabilita_Riga With {
                    .ValoreRiferimentoPrezzo = enum_EditImporto.PrezzoUnitario,
                    .PrezzoRiferitoA = enum_PrezzoLivello.Kg_Litri,
                    .Prezzo = prezzo_Unitario,
                    .PrezzoEffettivoKgL = prezzo_Unitario,
                    .PrezzoNetto = rigaMovDet.md.Prezzo_Unitario_Netto,
                    .ScontoModalita = rigaMovDet.md.Sconto_Modalita,
                    .ScontoBase = rigaMovDet.md.Sconto,
                    .ScontoAddiz1 = scontoAddiz1,
                    .ScontoAddiz2 = scontoAddiz2,
                    .ScontoAddiz3 = scontoAddiz3,
                    .ScontoCalcolato = scontoCalcolato,
                    .ForzaIva = False,
                    .CodIva = cod_iva,
                    .AliquotaIva = aliquota_Iva,
                    .ModuloGias = moduloGias,
                    .KgNetti = rigaMovDet.md.Qta_Extra_Totale,
                    .Degrado = CDec(rigaMovDet.md.Variazione),
                    .NumContenitori = rigaMovDet.md.Qta_Dettaglio1,
                    .NumImballi = rigaMovDet.md.Qta_Dettaglio2,
                    .NumConfezioni = numero_confezioni,
                    .Quantita = qta,
                    .ElemCod = rigaMovDet.md.Elem_Cod
                }

                ' Chiamo funzioni di ricalcolo prima della scrittura
                UtilityHelper.AggiornaDettagliEconomici(contabilita_riga, rigaMovDet.Lav_Cod)
                SistemaValoriRigaPerScrittura(contabilita_riga, rigaMovDet.Lav_Cod)

                ' riporto i valori di contabilita_riga in movimento_dettaglio
                rigaMovDet.md.Cod_Iva = contabilita_riga.CodIva
                rigaMovDet.md.Iva = contabilita_riga.Iva
                rigaMovDet.md.Prezzo_Livello = contabilita_riga.PrezzoRiferitoA
                rigaMovDet.md.Prezzo_Unitario = contabilita_riga.DB_Prezzo_Unitario
                rigaMovDet.md.Prezzo_Unitario_Netto = contabilita_riga.DB_Prezzo_Unitario_Netto
                rigaMovDet.md.Prezzo_Effettivo = contabilita_riga.PrezzoEffettivoKgL
                rigaMovDet.md.Imponibile = contabilita_riga.ImponibileTotale
                rigaMovDet.md.Imponibile_Netto = contabilita_riga.ImponibileTotaleNetto
                rigaMovDet.md.TempoCarenza = contabilita_riga.ValoreRiferimentoPrezzo

                rigaMovDet.md.Username_Modifica = objParametri.UsernameOperazione
                rigaMovDet.md.Data_Modifica = Now

                'dal.Movimenti_dettagli.Attach(rigaMovDet.md)
                'dal.ObjectStateManager.ChangeObjectState(rigaMovDet.md, EntityState.Modified)
                'dal.SaveChanges()


                ' Effettuo aggiornamento della riga di movimento_dettagli
                Dim objMovDetW = New AgronicaCoreContabDAL.Movimenti_Dettagli_W
                objMovDetW.ModificaPuntuale(Piva:=rigaMovDet.md.PIVA,
                                            Sa_Cod:=rigaMovDet.md.Sa_Cod,
                                            Id_Agenda:=rigaMovDet.md.Id_Agenda,
                                            Id_Mov:=rigaMovDet.md.Id_Mov,
                                            Id_Mov_Det:=rigaMovDet.md.Id_Mov_Det,
                                            Cod_Iva:=rigaMovDet.md.Cod_Iva,
                                            Iva:=rigaMovDet.md.Iva,
                                            Prezzo_Livello:=rigaMovDet.md.Prezzo_Livello,
                                            Prezzo_Unitario:=rigaMovDet.md.Prezzo_Unitario,
                                            Prezzo_Unitario_Netto:=rigaMovDet.md.Prezzo_Unitario_Netto,
                                            Prezzo_Effettivo:=rigaMovDet.md.Prezzo_Effettivo,
                                            Imponibile:=rigaMovDet.md.Imponibile,
                                            Imponibile_Netto:=rigaMovDet.md.Imponibile_Netto,
                                            Data_Modifica:=rigaMovDet.md.Data_Modifica,
                                            Username_Modifica:=rigaMovDet.md.Username_Modifica,
                                            objParametri:=objParametri)

            End If

        Catch ex As Exception
            risultato = ex.Message & If(ex.InnerException Is Nothing, "", " [" & ex.InnerException.Message & "]")
        End Try

        Return risultato

    End Function

#End Region

#Region "Listino"

    Public Shared Sub FormImpostaPrezzoListino(ByRef objListino As Contabilita_Listino,
                                               ByRef objContabDettaglio As Contabilita_Riga)

        'TODO: mUdm_Cod deve essere UdM o DB_Udm_Cod?!??
        'TODO: mUdm_Cod_Extra deve essere Udm_Cod_Extra o DB_Udm_Cod_Extra?!??
        'TODO: TxtQta_Extra deve essere Qta_Extra o DB_Qta_Extra?!??

        If objListino IsNot Nothing Then

            If objListino.UdmCod = objContabDettaglio.Udm_Cod_Extra Then
                '=============================================================================================================
                'Impostazione Prezzo Da UDM_COD_EXTRA
                '-------------------------------------------------------------------------------------------------------------
                'bPrezzoExtra = True
                objContabDettaglio.PrezzoRiferitoA = enum_PrezzoLivello.Kg_Litri
                objContabDettaglio.PrezzoEffettivoKgL = objListino.Prezzo

            ElseIf objListino.UdmCod = objContabDettaglio.UdM Then

                objContabDettaglio.Prezzo = objListino.Prezzo
                objContabDettaglio.PrezzoNetto = objContabDettaglio.Prezzo

                'Aggiornamento Prezzo Unitario Extra
                If IsNumeric(objContabDettaglio.Qta_Extra) Then
                    'objContabDettaglio.PrezzoEffettivoKgL.Locked = True 'Blocco Eventi

                    'Evito l'overflow
                    If CDec(objContabDettaglio.Qta_Extra) = 0 Then
                        objContabDettaglio.PrezzoEffettivoKgL = 0
                    Else
                        objContabDettaglio.PrezzoEffettivoKgL = ArrotondaVal_6(objContabDettaglio.Prezzo / objContabDettaglio.Qta_Extra)
                    End If

                    'objContabDettaglio.PrezzoEffettivoKgL.Locked = False

                End If

            End If

        End If

    End Sub

    Public Shared Sub FormImpostaScontoCondizioneListino(ByRef objListino As Contabilita_Listino,
                                                         ByRef objContabDettaglio As Contabilita_Riga)

        Dim bOk As Boolean = False

        'TODO: dovrei passarmi mSconto_Testo_Contatto
        Dim mSconto_Testo_Contatto As String = ""


        'TODO: TxtQta deve essere Quantita o DB_Qta?!??

        'Controllo Sconti
        If objListino IsNot Nothing AndAlso Trim(mSconto_Testo_Contatto) = "" Then

            objContabDettaglio.ScontoAddiz1 = 0

            'In caso si ts resetta lo sconto (viene usato lo sconto_add1 perché lo sconto di listino è associato al contatto).

            '=====================================================================================================
            'Condizione 1
            '-----------------------------------------------------------------------------------------------------
            Select Case objListino.ScontoCondizione1

                Case 0

                    bOk = True

                Case 1 'Quantità Minima

                    If objContabDettaglio.Quantita IsNot Nothing AndAlso
                       CDec(objContabDettaglio.Quantita) >= objListino.ScontoCondizioneValore1 Then
                        bOk = True
                    Else
                        bOk = False
                    End If

                Case Else '...

                    bOk = False

            End Select

            If bOk Then
                objContabDettaglio.ScontoAddiz1 = Math.Abs(objListino.ScontoAdd1)
            Else
                objContabDettaglio.ScontoAddiz1 = 0
            End If

            '=====================================================================================================
            'Condizione 2
            '-----------------------------------------------------------------------------------------------------
            Select Case objListino.ScontoCondizione2

                Case 0

                    bOk = True

                Case 1 'Quantità Minima

                    If objContabDettaglio.Quantita IsNot Nothing AndAlso
                       CDec(objContabDettaglio.Quantita) >= objListino.ScontoCondizioneValore2 Then
                        bOk = True
                    Else
                        bOk = False
                    End If

                Case Else '...

                    bOk = False

            End Select

            If bOk Then
                objContabDettaglio.ScontoAddiz2 = Math.Abs(objListino.ScontoAdd2)
            Else
                objContabDettaglio.ScontoAddiz2 = 0
            End If

            '=====================================================================================================
            'Condizione 3
            '-----------------------------------------------------------------------------------------------------
            Select Case objListino.ScontoCondizione3

                Case 0

                    bOk = True

                Case 1 'Quantità Minima

                    If objContabDettaglio.Quantita IsNot Nothing AndAlso
                       CDec(objContabDettaglio.Quantita) >= objListino.ScontoCondizioneValore3 Then
                        bOk = True
                    Else
                        bOk = False
                    End If

                Case Else '...

                    bOk = False

            End Select

            If bOk Then
                objContabDettaglio.ScontoAddiz3 = Math.Abs(objListino.ScontoAdd3)
            Else
                objContabDettaglio.ScontoAddiz3 = 0
            End If

        End If

    End Sub

    Public Shared Sub FormImpostaIva(ByVal Cod_Iva As Integer,
                                     ByRef objContabDettaglio As Contabilita_Riga,
                                     ByRef objContabImpostazioni As Contabilita_Impostazioni,
                                     Optional ByVal codIvaContatto As Integer = -1)

        '============================================================================================================
        'Impostazione Iva da Listino Prezzi
        '------------------------------------------------------------------------------------------------------------
        Select Case Cod_Iva

            Case -1 'Impostazione Default

                'Impostazione Codice Iva Default
                'FormCmbSeleziona(CmbIva, Cod_Iva_Default)
                objContabDettaglio.CodIva = Cod_Iva_Default(objContabDettaglio.ElemCod, objContabImpostazioni, codIvaContatto)

            Case Else

                'Impostazione Codice Iva
                'FormCmbSeleziona(CmbIva, Cod_Iva)
                objContabDettaglio.CodIva = Cod_Iva

        End Select

    End Sub

    Public Shared Sub FormImpostaTipoIvaListino(ByRef objListino As Contabilita_Listino,
                                                ByRef objContabDettaglio As Contabilita_Riga)

        Dim Ripristino_Sconto As Decimal?

        'TODO: dovrei passarmi mSconto_Testo_Contatto
        Dim mSconto_Testo_Contatto As String = ""

        'TODO: TxtQta deve essere Quantita o DB_Qta?!??

        If objListino IsNot Nothing Then

            '============================================================================================================
            'Impostazione Tipo Iva
            '------------------------------------------------------------------------------------------------------------
            Select Case objListino.TipoIvaDet

                Case enum_TipoIVAListino.IVA_Sconto_Inclusi

                    'Impostazione del prezzo ivato
                    objContabDettaglio.Quantita = If(objContabDettaglio.Quantita, 1)

                    objContabDettaglio.ImportoUnitario = objContabDettaglio.Prezzo
                    objContabDettaglio.ValoreRiferimentoPrezzo = enum_EditImporto.Importo_Unitario

                    'ChkPrezzo.value = ssChecked
                    'mbBloccoImpostazioneCheckPrezzo = True

                Case enum_TipoIVAListino.IVA_Esclusa

                    'Controllo impostazione già settata su chkprezzo_extra
                    If objContabDettaglio.PrezzoRiferitoA <> enum_PrezzoLivello.Kg_Litri Then
                        'ChkPrezzo.value = ssChecked
                        objContabDettaglio.ValoreRiferimentoPrezzo = enum_EditImporto.PrezzoUnitario
                    End If

                Case enum_TipoIVAListino.IVA_Inclusa

                    'Impostazione del prezzo ivato
                    objContabDettaglio.Quantita = If(objContabDettaglio.Quantita, 1)

                    If Trim(mSconto_Testo_Contatto) = "" Then
                        'Reset sconto
                        Ripristino_Sconto = objContabDettaglio.ScontoBase
                        objContabDettaglio.ScontoBase = 0
                        objContabDettaglio.ScontoAddiz1 = 0
                        objContabDettaglio.ScontoAddiz2 = 0
                        objContabDettaglio.ScontoAddiz3 = 0
                    End If

                    objContabDettaglio.ImportoUnitario = objContabDettaglio.Prezzo
                    objContabDettaglio.ValoreRiferimentoPrezzo = enum_EditImporto.Importo_Unitario

                    'Giulia - 31/07/2018: se sono iva inclusa il check deve essere su importo unitario, non su prezzo unitario
                    'ChkPrezzo.value = ssChecked
                    'mbBloccoImpostazioneCheckPrezzo = True


                    If Trim(mSconto_Testo_Contatto) = "" Then

                        'riapplico lo sconto
                        objContabDettaglio.ScontoBase = Ripristino_Sconto

                        FormImpostaScontoCondizioneListino(objListino, objContabDettaglio)

                    End If

            End Select

        End If

    End Sub

    Private Shared Function Cod_Iva_Default(ByVal elemCod As Integer,
                                            ByRef objContabImpostazioni As Contabilita_Impostazioni,
                                            Optional ByVal codIvaContatto As Integer = -1) As Integer

        Dim codIvaDefault As Integer = 0
        Dim mCod_Iva_Default_Info As String = ""

        Select Case codIvaContatto

            Case -1

                'Impostazione Codice Iva Default
                codIvaDefault = objContabImpostazioni.SUPERUSER_COD_IVA_DEFAULT 'Inizializzazione
                mCod_Iva_Default_Info = "Impostazione Generica da Strumenti-->Opzioni."

                'Impostazione per Categoria
                If objContabImpostazioni.ConfigurazioneCategorie IsNot Nothing AndAlso objContabImpostazioni.ConfigurazioneCategorie.Count > 0 Then
                    Dim obj = objContabImpostazioni.ConfigurazioneCategorie.FirstOrDefault(Function(x) x.ElemCod = elemCod)
                    If obj IsNot Nothing AndAlso obj.IvaCodDefault <> -1 Then
                        codIvaDefault = obj.IvaCodDefault
                        mCod_Iva_Default_Info = "Impostazione Categoria in Strumenti-->Opzioni."
                    End If
                End If


            Case Else

                codIvaDefault = codIvaContatto
                mCod_Iva_Default_Info = "Impostazione da Anagrafica Contatto."

        End Select

        Return codIvaDefault

    End Function

#End Region

#Region "Verifiche Preliminari"

    Private Shared Function CheckPropertyRigaContabilita(ByRef objContabRiga As Contabilita_Riga,
                                                         ByVal tipoOperazione As enum_TipoOperazioneDB,
                                                         ByRef objParametriServer As AgronicaCoreParametri
                                                         ) As Boolean

        Const nomeRoutine = "ContabilitaHelper.CheckPropertyRigaContabilita()"
        Dim stringaErrore As String = ""

        Try

            UtilityHelper.ControllaStringEmpty(objContabRiga, Nothing, stringaErrore, "Piva")
            'UtilityHelper.ControllaStringEmpty(objContabRiga, objParametriServer.UsernameOperazione, stringaErrore, "UsernameModifica")

            If tipoOperazione = enum_TipoOperazioneDB.Modifica OrElse
               tipoOperazione = enum_TipoOperazioneDB.Cancellazione Then
                UtilityHelper.ControllaStringEmpty(objContabRiga, Nothing, stringaErrore, "ChiaveRiga")
                UtilityHelper.ControllaNumZero(objContabRiga, Nothing, stringaErrore, "IdAgenda")
                UtilityHelper.ControllaNumZero(objContabRiga, Nothing, stringaErrore, "IdMov")
                UtilityHelper.ControllaNumZero(objContabRiga, Nothing, stringaErrore, "IdMovDet")
            End If

            If tipoOperazione = enum_TipoOperazioneDB.Scrittura OrElse tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                'UtilityHelper.ControllaNumZero(objContabRiga, Nothing, stringaErrore, "LavCod")
                UtilityHelper.ControllaNumZero(objContabRiga, Nothing, stringaErrore, "ElemCod")
                UtilityHelper.ControllaPresenza(objContabRiga, Nothing, stringaErrore, "jolly_int")

                'UtilityHelper.ControllaNumZero(objContabRiga, Nothing, stringaErrore, "DestinazioneScarico")
                'UtilityHelper.ControllaNumZero(objContabRiga, Nothing, stringaErrore, "DestinazioneCarico")

                UtilityHelper.ControllaPresenza(objContabRiga, 0, stringaErrore, "OrdineDet")
                UtilityHelper.ControllaPresenza(objContabRiga, 0, stringaErrore, "SaCod")
                UtilityHelper.ControllaPresenza(objContabRiga, 0, stringaErrore, "MatCod")
                UtilityHelper.ControllaPresenza(objContabRiga, 0, stringaErrore, "MatCodAlias")
                UtilityHelper.ControllaPresenza(objContabRiga, 0, stringaErrore, "ProCod")
                UtilityHelper.ControllaPresenza(objContabRiga, "", stringaErrore, "MatDes")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "N")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "P2O5")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "K2O")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "Cu")
                UtilityHelper.ControllaPresenza(objContabRiga, 0, stringaErrore, "Pendente")
                UtilityHelper.ControllaPresenza(objContabRiga, "", stringaErrore, "Lotto")
                UtilityHelper.ControllaPresenza(objContabRiga, 0, stringaErrore, "UdM")
                UtilityHelper.ControllaPresenza(objContabRiga, 0, stringaErrore, "Udm_Cod_Extra")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "Quantita")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "NumConfezioni")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "NumContenitori")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "NumImballi")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "KgLordi")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "Tara")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "KgNetti")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "Degrado")
                UtilityHelper.ControllaPresenza(objContabRiga, "", stringaErrore, "Extra_Str")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "Tipo_Associazione")



                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "ImponibileTotale")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "ImponibileTotaleNetto")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "Iva")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "IvaIndetraibile")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "Prezzo")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "PrezzoNetto")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "PrezzoEffettivoKgL")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "Qta_Extra")

                'TODO: Check ANNO
                'UtilityHelper.ControllaPresenza(objContabRiga, 0, stringaErrore, "Anno")
                UtilityHelper.ControllaPresenza(objContabRiga, 0, stringaErrore, "ContoEconomico")
                UtilityHelper.ControllaPresenza(objContabRiga, 0, stringaErrore, "ContoPatrimoniale")

                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "ProvvigionePercAgente")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "ProvvigioneAgente")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "ProvvigionePercCapoArea")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "ProvvigioneCapoArea")

                UtilityHelper.ControllaPresenza(objContabRiga, 0, stringaErrore, "Fornitore")

                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "Riscontrati_PesoNetto")
                UtilityHelper.ControllaPresenza(objContabRiga, 0D, stringaErrore, "Riscontrati_PesoLordo")


                'TODO: per casi molto particolari setto i valori qui:
                If objContabRiga.ElemCod IsNot Nothing AndAlso objContabRiga.ElemCod = RIGA_DESCRIZIONE_LIBERA Then
                    UtilityHelper.ControllaStringEmpty(objContabRiga, Nothing, stringaErrore, "BeniStrumentali")
                    objContabRiga.ProCod = 0
                    objContabRiga.MatCod = 0
                    objContabRiga.UdM = 0
                    objContabRiga.Quantita = 0D
                    objContabRiga.Contabilizzato = CONTABILE '????
                    'objContabRiga.Pendente = 0 '??? deve arrivare da fuori
                    objContabRiga.CodIva = -1
                    objContabRiga.jolly_int = MagazzinoNONMovimentato
                    objContabRiga.PrezzoRiferitoA = enum_PrezzoLivello.Udm_principale
                    objContabRiga.ValoreRiferimentoPrezzo = enum_EditImporto.PrezzoUnitario
                End If

            End If

            If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                'UtilityHelper.ControllaNumZero(objContabRiga, Nothing, stringaErrore, "IdMov")
                UtilityHelper.ControllaPresenza(objContabRiga, Nothing, stringaErrore, "DataOraUltimaLettura")
                'in realtà una volta fissato non lo dovrò più modificare ma mi serve perché così so che tipo di movimento sto aggiornando
                '(potrebbero esserci cose diverse da fare a seconda del lav cod)
                'UtilityHelper.ControllaNumZero(objContabRiga, Nothing, stringaErrore, "LavCod")
            End If

            If Not String.IsNullOrEmpty(stringaErrore) Then
                Throw New Exception(stringaErrore)
            End If

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return True

    End Function

#End Region

    Public Function ForzaEvasioneRigheOrdine(ByVal piva As String,
                                             ByVal idAgenda As Integer,
                                             ByVal listDettagli As List(Of Integer),
                                             ByVal forzaEvasione As Boolean,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Boolean

        Const nomeRoutine = "ForzaEvasioneRigheOrdine()"
        Dim xRisp As Boolean = False
        Dim objMovW As New AgronicaCoreContabDAL.Movimenti_Dettagli_W

        Try

            Dim filtroAggiuntivo As String = " Elem_Cod <> " & RIGA_DESCRIZIONE_LIBERA

            Dim listIdMovDetStr As String = ""
            If listDettagli.Count > 0 Then
                listIdMovDetStr = String.Join(",", listDettagli.ToArray())
            End If

            If listIdMovDetStr <> "" Then
                filtroAggiuntivo &= " AND Id_Mov_Det IN ( " & listIdMovDetStr & ") "
            End If

            Dim contabilizzato As Integer?
            If forzaEvasione Then
                contabilizzato = CONTABILE_EVASO_FORZATAMENTE
            Else
                contabilizzato = CONTABILE
            End If

            xRisp = objMovW.ModificaPuntuale(piva, 0, idAgenda, 0, 0, objParametri,
                                             Contabilizzato:=contabilizzato,
                                             xFiltroAggiuntivo:=filtroAggiuntivo)

        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return xRisp

    End Function

    ''' <summary>
    ''' Viene utilizzata con l'impostazione Conferimento_Da_Raccolta attiva, quindi viene dato per assodato che sia attiva anche l'impostazione Raccolta_Con_Carico_Magazzino
    ''' se l'impostazione "Sovrascrivi peso raccolta con peso conferimento" è a true, ne aggiorna le qta calcolandole dal conferimento e
    ''' se l'impostazione "Genera scarico su azienda agricola in caso di carichi da raccolta collegati a conferimento" è a true crea anche relativo mov di scarico
    ''' </summary>
    ''' <returns></returns>
    Private Function AggiornaRaccoltaAziendaCampagna(objContabDettaglio As Contabilita_Riga, objContabTestata As Contabilita_Testata, ByVal operazioneSvoltaSulConferimento As enum_TipoOperazioneDB) As Contabilita_Output

        Const nomeRoutine = "ContabilitaHelper_Dettaglio.AggiornaRaccoltaAziendaCampagna()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim objOutputAgendaScarico As New Contabilita_Output() With {
            .Risultato = True
        }

        'Setup "Sovrascrivi peso raccolta con peso conferimento", se è false non occorre aggiornare le quantità
        Dim objImpreseImpostazioni As New Imprese_Impostazioni_R
        Dim setupSovrascriviPesoRaccolta As Boolean = True

        Dim helperOpAgenda As New Agenda_Operazione_Helper()
        Dim helperMovDettagli As New Agenda_Movimenti_Dettagli_Helper()
        Dim helperMovDest As New Agenda_Movimenti_Destinazioni_Helper()

        Dim listaMovDetCarichi As New List(Of Movimento_Dettaglio)()
        Dim listaDestRaccolte As New List(Of Movimento_Destinazione)()
        Dim listaAgendeRaccolte As New List(Of Raccolta_Aggiorna)()

        Dim listaOperazioniAgenda As New Dictionary(Of Integer, Operazione_Agenda)()

        Try
            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            'Leggo l'impostazione con la piva dell'impresa ma non i centri aziendali
            Dim strSetupSovrascriviPesoRaccolta = objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser(
                objContabDettaglio.PivaConferente, New List(Of Integer)({0}), enum_Impostazioni_Utenti.Aggiorna_Peso_Raccolta_Da_Conf,
                "", _objParametriUtenti, _objParametriServer)

            If strSetupSovrascriviPesoRaccolta = "0" Then
                setupSovrascriviPesoRaccolta = False
            End If

            For Each detSceltoDaUtente As Movimento_Dettaglio In objContabDettaglio.Raccolte

                Dim opAgRacc As Operazione_Agenda

                'Leggo tutti i dati di ogni Agenda di Raccolta
                If listaOperazioniAgenda.ContainsKey(detSceltoDaUtente.Id_Agenda) Then
                    opAgRacc = listaOperazioniAgenda(detSceltoDaUtente.Id_Agenda)
                Else
                    opAgRacc = helperOpAgenda.Leggi(objContabDettaglio.PivaConferente, 0, detSceltoDaUtente.Id_Agenda, 0, _objParametriServer)
                    listaOperazioniAgenda.Add(detSceltoDaUtente.Id_Agenda, opAgRacc)
                End If

                Dim movRaccolta As Movimento = opAgRacc.Movimenti.First(Function(mov) mov.Cau_Mov = CAU_RILIEVO_RACCOLTA)
                Dim movCarico As Movimento = opAgRacc.Movimenti.First(Function(mov) mov.Cau_Mov = CAU_CARICO)

                Dim movDetRaccolta As Movimento_Dettaglio
                Dim movDetCarico As Movimento_Dettaglio

                If detSceltoDaUtente.Cau_Mov = CAU_RILIEVO_RACCOLTA Then
                    'Caso di raccolta che era stata creata senza carico di magazzino (quest'ultimo l'ho scritto su database prima di arrivare qui)
                    '-> parto dal dettaglio di campagna e devo ricavare quello di carico
                    movDetRaccolta = movRaccolta.Movimenti_Dettagli.First(Function(det) det.Id_Mov_Det = detSceltoDaUtente.Id_Mov_Det)
                    'in questo caso ho creato un solo dettaglio di carico, non dividendo per Cod_Progetto, quindi è sufficiente
                    'recuperare il dettaglio di carico che corrisponde per mat_cod e lotto
                    movDetCarico = movCarico.Movimenti_Dettagli.First(Function(det) det.Mat_Cod = movDetRaccolta.Mat_Cod AndAlso det.Lotto = movDetRaccolta.Lotto)
                Else
                    'Caso di raccolta con carico di magazzino
                    '-> parto dal dettaglio di carico e devo ricavare quello di campagna
                    movDetCarico = movCarico.Movimenti_Dettagli.First(Function(det) det.Id_Mov_Det = detSceltoDaUtente.Id_Mov_Det)
                    'Il dettaglio di raccolta corrispondente è quello con stessi mat_cod e lotto.
                    'In caso i dettagli di carico sono divisi per cod_progetto, questo dettaglio di raccolta si riferisce a più carichi,
                    'altrimenti è univoco
                    movDetRaccolta = movRaccolta.Movimenti_Dettagli.First(Function(det) det.Mat_Cod = movDetCarico.Mat_Cod AndAlso det.Lotto = movDetCarico.Lotto)
                End If

                listaMovDetCarichi.Add(movDetCarico)

                If setupSovrascriviPesoRaccolta = True Then
                    Dim listaDestinazioniCampagnaSpecifiche As List(Of Movimento_Destinazione)

                    If movDetCarico.Cod_Progetto = 0 Then
                        listaDestinazioniCampagnaSpecifiche = movDetRaccolta.Movimenti_Destinazioni
                    Else
                        'Ottengo una lista con un solo elemento
                        listaDestinazioniCampagnaSpecifiche = movDetRaccolta.Movimenti_Destinazioni.Where(Function(d) d.Progetto_Cod = movDetCarico.Cod_Progetto).ToList()
                    End If

                    listaAgendeRaccolte.Add(New Raccolta_Aggiorna() With {
                        .Id_Agenda = opAgRacc.Id_Agenda,
                        .Movimento_Campagna = movRaccolta,
                        .Movimento_Carico = movCarico,
                        .Mov_Dettagli_Campagna = movDetRaccolta,
                        .Mov_Dettagli_Carico = movDetCarico,
                        .Mov_Destinazioni_Campagna = listaDestinazioniCampagnaSpecifiche,
                        .Mov_Destinazioni_Carico = movDetCarico.Movimenti_Destinazioni.First
                    })

                    For Each dest In listaDestinazioniCampagnaSpecifiche
                        'La funzione Contains utilizza come funzione di confronto l'override della funzione Equals
                        'della classe Movimento_Destinazione
                        If Not listaDestRaccolte.Contains(dest) Then
                            listaDestRaccolte.Add(dest)
                        End If
                    Next
                End If

            Next

            'Per l'aggiornamento delle quantità in caso di Cod_Progetto si tiene conto che:
            '1) La quantità del dettaglio di raccolta è la somma delle quantità dei relativi dettagli di carico
            '2) L'utente seleziona dei dettagli di carico, l'aggiornamento deve influire solo su quelli selezionati
            If setupSovrascriviPesoRaccolta = True Then

                Dim raggruppaPerQta As Boolean = listaMovDetCarichi.All(Function(movDet) movDet.Qta <> 0)

                Dim groupQtaRaccolteAg As List(Of Raccolta_Group_Qta)

                If raggruppaPerQta = True Then
                    'Ripartiziono per le qta indicate nelle raccolte, in questo modo gestisco sia i casi di ripartizione automatica sugli ettari o piante sia sui casi di ripartizione manuale
                    groupQtaRaccolteAg = GroupRaccolteByQta(listaDestRaccolte, objContabDettaglio)
                Else
                    'Nel caso di raccolte che erano nate senza carico di magazzino, non ho delle qta, quindi ripartiziono in base agli ettari o numero piante impiegate
                    groupQtaRaccolteAg = GroupRaccolteByEttari(listaDestRaccolte, objContabDettaglio)
                End If

                'Eseguo queste funzioni prima dei cicli perché ho bisogno dei valori di qta del database e non quelli aggiornati
                Dim groupQtaCarichiPerRaccolta = GroupRaccolteQtaDettagli(listaAgendeRaccolte)
                Dim groupQtaDettagliProdottoLotto = GroupRaccolteQtaDettagliMatCodLotto(listaAgendeRaccolte)

                Dim elenco_nuoveSommeQtaDettagliProdottoLotto As New Dictionary(Of String, Decimal)()

                For index = 0 To listaAgendeRaccolte.Count - 1
                    Dim raccAgg = listaAgendeRaccolte(index) 'raccAgg sta per raccolta da aggiornare

                    Dim qtaRaccolteAg = groupQtaRaccolteAg.First(Function(d) d.Id_Agenda = raccAgg.Id_Agenda)

                    Dim sommaQtaDettagli = groupQtaCarichiPerRaccolta.First(Function(elem) elem.Id_Agenda = raccAgg.Id_Agenda).QtaDetCarico

                    'Divido la qta del dettaglio che sto ciclando con la qta somma fra questa e le qta degli altri dettagli della stessa agenda che devo aggiornare,
                    'allo scopo di ottenere la quota distribuzione di questo dettaglio rispetto al totale, il risultato lo moltiplico con la qta da assegnare a questa agenda,
                    'di modo da distribuirla proporzionalmente.
                    Dim qtaPerDettaglio = raccAgg.Mov_Dettagli_Carico.Qta / sommaQtaDettagli * qtaRaccolteAg.NuovaQtaRaccoltaConf 'Math.Round(raccAgg.Mov_Dettagli_Carico.Qta / sommaQtaDettagli * qtaRaccolteAg.NuovaQtaRaccoltaConf, 2)

                    If raccAgg.Mov_Dettagli_Carico.Cod_Progetto = 0 Then

                        If Not (qtaRaccolteAg.VecchiaQtaRaccolta = qtaRaccolteAg.NuovaQtaRaccoltaConf AndAlso qtaRaccolteAg.QuotaDistribuzione = 1) Then

                            For Each dest As Movimento_Destinazione In raccAgg.Mov_Destinazioni_Campagna

                                dest.Qta = dest.QuotaDistribuzione * qtaPerDettaglio 'Math.Round(dest.QuotaDistribuzione * qtaPerDettaglio, 2)
                                helperMovDest.ModificaPuntuale(dest.Piva, dest.Sa_Cod, dest.Id_Agenda, dest.Id_Mov, dest.Id_Mov_Det, dest.Appezza, dest.Id_Destinazione, _objParametriServer, dest)
                            Next

                            raccAgg.Mov_Destinazioni_Carico.Qta = qtaPerDettaglio

                            helperMovDest.ModificaPuntuale(
                                raccAgg.Mov_Destinazioni_Carico.Piva,
                                raccAgg.Mov_Destinazioni_Carico.Sa_Cod,
                                raccAgg.Mov_Destinazioni_Carico.Id_Agenda,
                                raccAgg.Mov_Destinazioni_Carico.Id_Mov,
                                raccAgg.Mov_Destinazioni_Carico.Id_Mov_Det,
                                raccAgg.Mov_Destinazioni_Carico.Appezza,
                                raccAgg.Mov_Destinazioni_Carico.Id_Destinazione,
                                _objParametriServer,
                                raccAgg.Mov_Destinazioni_Carico)


                            raccAgg.Mov_Dettagli_Campagna.Qta = qtaPerDettaglio
                            raccAgg.Mov_Dettagli_Campagna.Qta_Extra_Totale = qtaPerDettaglio

                            helperMovDettagli.ModificaPuntuale(
                                raccAgg.Mov_Dettagli_Campagna.Piva,
                                raccAgg.Mov_Dettagli_Campagna.Sa_Cod,
                                raccAgg.Mov_Dettagli_Campagna.Id_Agenda,
                                raccAgg.Mov_Dettagli_Campagna.Id_Mov,
                                raccAgg.Mov_Dettagli_Campagna.Id_Mov_Det,
                                _objParametriServer,
                                raccAgg.Mov_Dettagli_Campagna)


                            raccAgg.Mov_Dettagli_Carico.Qta = qtaPerDettaglio

                            helperMovDettagli.ModificaPuntuale(
                                raccAgg.Mov_Dettagli_Carico.Piva,
                                raccAgg.Mov_Dettagli_Carico.Sa_Cod,
                                raccAgg.Mov_Dettagli_Carico.Id_Agenda,
                                raccAgg.Mov_Dettagli_Carico.Id_Mov,
                                raccAgg.Mov_Dettagli_Carico.Id_Mov_Det,
                                _objParametriServer,
                                raccAgg.Mov_Dettagli_Carico)

                        End If

                    Else

                        'Aggiorno la sola destinazione di campagna che punta allo stesso esercizio del dettaglio di carico in esame 
                        Dim destAgg = raccAgg.Mov_Destinazioni_Campagna.First(Function(dest) dest.Progetto_Cod = raccAgg.Mov_Dettagli_Carico.Cod_Progetto)
                        destAgg.Qta = qtaPerDettaglio
                        helperMovDest.ModificaPuntuale(destAgg.Piva, destAgg.Sa_Cod, destAgg.Id_Agenda, destAgg.Id_Mov, destAgg.Id_Mov_Det, destAgg.Appezza, destAgg.Id_Destinazione, _objParametriServer, destAgg)


                        raccAgg.Mov_Destinazioni_Carico.Qta = qtaPerDettaglio
                        helperMovDest.ModificaPuntuale(
                            raccAgg.Mov_Destinazioni_Carico.Piva,
                            raccAgg.Mov_Destinazioni_Carico.Sa_Cod,
                            raccAgg.Mov_Destinazioni_Carico.Id_Agenda,
                            raccAgg.Mov_Destinazioni_Carico.Id_Mov,
                            raccAgg.Mov_Destinazioni_Carico.Id_Mov_Det,
                            raccAgg.Mov_Destinazioni_Carico.Appezza,
                            raccAgg.Mov_Destinazioni_Carico.Id_Destinazione,
                            _objParametriServer,
                            raccAgg.Mov_Destinazioni_Carico)

                        'La qta del dettaglio di campagna è la somma delle qta dei dettagli di carico con stesso mat_cod/lotto,
                        'quindi per aggiornarla devo sapere la somma aggiornata di questi, di conseguenza la calcolo e poi aggiorno in un nuovo ciclo
                        Dim chiave = raccAgg.Id_Agenda & "|" & raccAgg.Mov_Dettagli_Carico.Mat_Cod & "|" & raccAgg.Mov_Dettagli_Carico.Lotto

                        If elenco_nuoveSommeQtaDettagliProdottoLotto.ContainsKey(chiave) Then
                            elenco_nuoveSommeQtaDettagliProdottoLotto(chiave) += qtaPerDettaglio
                        Else
                            elenco_nuoveSommeQtaDettagliProdottoLotto.Add(chiave, qtaPerDettaglio)
                        End If

                        raccAgg.Mov_Dettagli_Carico.Qta = qtaPerDettaglio
                        helperMovDettagli.ModificaPuntuale(
                                raccAgg.Mov_Dettagli_Carico.Piva,
                                raccAgg.Mov_Dettagli_Carico.Sa_Cod,
                                raccAgg.Mov_Dettagli_Carico.Id_Agenda,
                                raccAgg.Mov_Dettagli_Carico.Id_Mov,
                                raccAgg.Mov_Dettagli_Carico.Id_Mov_Det,
                                _objParametriServer,
                                raccAgg.Mov_Dettagli_Carico)

                    End If
                Next

                If elenco_nuoveSommeQtaDettagliProdottoLotto.Count > 0 Then

                    'Secondo ciclo per aggiornare le qta dei dettagli di campagna dei cod_progetti,
                    'perché mi serve nel primo ciclo calcolare la somma nuova dei dettagli di carico
                    For index = 0 To listaAgendeRaccolte.Count - 1

                        Dim raccAgg = listaAgendeRaccolte(index) 'raccAgg sta per raccolta da aggiornare

                        If raccAgg.Mov_Dettagli_Carico.Cod_Progetto <> 0 Then
                            Dim sommaQtaDettagliProdottoLotto = groupQtaDettagliProdottoLotto.First(Function(elem)
                                                                                                        Return elem.Id_Agenda = raccAgg.Id_Agenda AndAlso
                                                                                                        elem.Mat_Cod = raccAgg.Mov_Dettagli_Carico.Mat_Cod AndAlso
                                                                                                        elem.Lotto = raccAgg.Mov_Dettagli_Carico.Lotto
                                                                                                    End Function).QtaDetCarico

                            'La qta del dettaglio di campagna è la somma delle qta dei dettagli di carico con stesso mat_cod/lotto,
                            'quindi per aggiornarla prendo il valore che aveva, ne rimuovo il valore sommato dei dettagli da aggiornare (la somma dei vecchi valori)
                            'e ne aggiungo il valore nuovo assegnato a questa agenda
                            'Nota: per come ho fatto l'oggetto listaAgendeRaccolte una stessa agenda compare più volte se l'utente ne ha selezionato più dettagli,
                            'di conseguenza l'aggiornamento del dettaglio di campagna può essere ripetuto dal ciclo, anche se non sarebbe necessario
                            Dim chiave = raccAgg.Id_Agenda & "|" & raccAgg.Mov_Dettagli_Carico.Mat_Cod & "|" & raccAgg.Mov_Dettagli_Carico.Lotto
                            Dim nuovaSommaQtaDettagliProdottoLotto = elenco_nuoveSommeQtaDettagliProdottoLotto(chiave)

                            Dim nuovaQtaDetCampagna = raccAgg.Mov_Dettagli_Campagna.Qta - sommaQtaDettagliProdottoLotto + nuovaSommaQtaDettagliProdottoLotto 'Math.Round(raccAgg.Mov_Dettagli_Campagna.Qta - sommaQtaDettagliProdottoLotto + nuovaSommaQtaDettagliProdottoLotto, 2)
                            raccAgg.Mov_Dettagli_Campagna.Qta = nuovaQtaDetCampagna
                            raccAgg.Mov_Dettagli_Campagna.Qta_Extra_Totale = nuovaQtaDetCampagna

                            helperMovDettagli.ModificaPuntuale(
                                raccAgg.Mov_Dettagli_Campagna.Piva,
                                raccAgg.Mov_Dettagli_Campagna.Sa_Cod,
                                raccAgg.Mov_Dettagli_Campagna.Id_Agenda,
                                raccAgg.Mov_Dettagli_Campagna.Id_Mov,
                                raccAgg.Mov_Dettagli_Campagna.Id_Mov_Det,
                                _objParametriServer,
                                raccAgg.Mov_Dettagli_Campagna)
                        End If
                    Next
                End If

            End If

            'If setupGeneraScaricoAzAgricola = True Then
            Dim bollaEmessaIdAgenda As Integer

            If operazioneSvoltaSulConferimento = enum_TipoOperazioneDB.Modifica AndAlso objContabDettaglio.ListRifMovDettaglio.Count > 0 Then
                Dim rifCancellatoBollaEmessa = objContabDettaglio.ListRifMovDettaglio.Where(Function(detRif) detRif.Lav_Cod_Rif = LAVCOD_BOLLA_EMESSA AndAlso detRif.Tipo_Associazione = 2).ToList()
                bollaEmessaIdAgenda = If(rifCancellatoBollaEmessa.Count > 0, rifCancellatoBollaEmessa.First().Id_Agenda_Rif, 0)
            End If

            objOutputAgendaScarico = ScriviScaricoRaccolteAziendaAgricola(operazioneSvoltaSulConferimento, objContabTestata.Piva, objContabTestata.IdAgenda, objContabDettaglio.IdMov, objContabDettaglio.IdMovDet, bollaEmessaIdAgenda, listaMovDetCarichi)
            'End If

            If objOutputAgendaScarico.Risultato = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objOutputAgendaScarico

    End Function

    Private Function GroupRaccolteByQta(ByRef listaDestRaccolte As List(Of Movimento_Destinazione), objContabDettaglio As Contabilita_Riga) As List(Of Raccolta_Group_Qta)

        'Ripartiziono per le qta indicate nelle raccolte, in questo modo gestisco sia i casi di ripartizione automatica sugli ettari o piante sia sui casi di ripartizione manuale
        'In pratica objContabDettaglio.DB_Qta, che è la quantità indicata nella riga di conferimento, è quella da spartire fra gli n dettagli
        'appartenenti ad m raccolte scelti dall'utente
        'Quindi la divisione fra VecchiaQtaRaccolta e qtaTotaleRaccolte serve a capire in che relazione di forza sono fra loro le agende
        'NuovaQtaRaccoltaConf è il nuovo totale da assegnare alla singola agenda

        Dim qtaTotaleRaccolte = listaDestRaccolte.Sum(Function(dest) dest.Qta)

        'Math.Round(CDec(g.Sum(Function(d) d.Qta) / qtaTotaleRaccolte * objContabDettaglio.DB_Qta), 2)
        Dim groupQtaRaccolteAg =
                    (From movDest In listaDestRaccolte
                     Group movDest By
                         _idAgenda = movDest.Id_Agenda
                         Into g = Group
                     Select New Raccolta_Group_Qta With {
                         .Id_Agenda = _idAgenda,
                         .VecchiaQtaRaccolta = g.Sum(Function(d) d.Qta),
                         .QuotaDistribuzione = g.Sum(Function(d) d.Qta) / qtaTotaleRaccolte,
                         .NuovaQtaRaccoltaConf = CDec(g.Sum(Function(d) d.Qta) / qtaTotaleRaccolte * objContabDettaglio.DB_Qta)
                     }).ToList()

        Return groupQtaRaccolteAg

    End Function

    Private Function GroupRaccolteByEttari(ByRef listaDestRaccolte As List(Of Movimento_Destinazione), objContabDettaglio As Contabilita_Riga) As List(Of Raccolta_Group_Qta)

        'Nel caso di raccolte Fast non ho delle qta, quindi ripartiziono in base agli ettari o numero piante impiegate
        Dim supTotImpianti = listaDestRaccolte.Sum(Function(dest) dest.Qta2)

        'Math.Round(CDec(g.Sum(Function(d) d.Qta2) / supTotImpianti * objContabDettaglio.DB_Qta), 2)
        Dim groupQtaRipartizionata =
                    (From movDest In listaDestRaccolte
                     Group movDest By
                         _idAgenda = movDest.Id_Agenda
                         Into g = Group
                     Select New Raccolta_Group_Qta With {
                         .Id_Agenda = _idAgenda,
                         .VecchiaQtaRaccolta = g.Sum(Function(d) d.Qta2),
                         .QuotaDistribuzione = g.Sum(Function(d) d.Qta2) / supTotImpianti,
                         .NuovaQtaRaccoltaConf = CDec(g.Sum(Function(d) d.Qta2) / supTotImpianti * objContabDettaglio.DB_Qta)
                     }).ToList()

        Return groupQtaRipartizionata

    End Function

    Private Function GroupRaccolteQtaDettagli(ByRef listaAgendeRaccolte As List(Of Raccolta_Aggiorna)) As List(Of Raccolta_Group_Qta)

        Dim listaQtaProgetto =
                    (From ag In listaAgendeRaccolte
                     Group ag By
                         _idAgenda = ag.Id_Agenda
                         Into g = Group
                     Select New Raccolta_Group_Qta With {
                         .Id_Agenda = _idAgenda,
                         .QtaDetCarico = g.Sum(Function(elem) elem.Mov_Dettagli_Carico.Qta)
                     }).ToList()

        Return listaQtaProgetto

    End Function

    Private Function GroupRaccolteQtaDettagliMatCodLotto(ByRef listaAgendeRaccolte As List(Of Raccolta_Aggiorna)) As List(Of Raccolta_Group_Qta)

        Dim listaQtaProgetto =
                    (From ag In listaAgendeRaccolte
                     Group ag By
                         _idAgenda = ag.Id_Agenda,
                         _matCod = ag.Mov_Dettagli_Carico.Mat_Cod,
                         _lotto = ag.Mov_Dettagli_Carico.Lotto
                         Into g = Group
                     Select New Raccolta_Group_Qta With {
                         .Id_Agenda = _idAgenda,
                         .Mat_Cod = _matCod,
                         .Lotto = _lotto,
                         .QtaDetCarico = g.Sum(Function(elem) elem.Mov_Dettagli_Carico.Qta)
                     }).ToList()

        Return listaQtaProgetto

    End Function



    ''' <summary>
    ''' A partire da un dettaglio di raccolta, scrive su database un relativo dettaglio di carico
    ''' Viene utilizzata con l'impostazione Conferimento_Da_Raccolta attiva, quindi viene dato per assodato che sia attiva anche l'impostazione Raccolta_Con_Carico_Magazzino
    ''' </summary>
    ''' <param name="detRaccolta">È il dettaglio del CAU_RILIEVO_RACCOLTA</param>
    ''' <param name="dettaglioConf"></param>
    Private Function ScriviCaricoRaccoltaAziendaAgricola(ByRef detRaccolta As Movimento_Dettaglio, dettaglioConf As Contabilita_Riga) As Movimento_Dettaglio

        Const nomeRoutine = "ContabilitaHelper_Dettaglio.ScriviCaricoRaccoltaAziendaAgricola()"

        Dim objMovDettagliC As Movimento_Dettaglio

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim helperOpAgenda As New Agenda_Operazione_Helper()
        Dim dalFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R()
        Dim objSequenze As New Agro_Sequenze()
        Dim handleImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read()

        Try
            'Nella variabile detRaccolta, ho solo i dati identificativi del record, avendo bisogno anche di altri, effettuo la lettura
            Dim opAgRacc As Operazione_Agenda = helperOpAgenda.Leggi(detRaccolta.Piva, 0, detRaccolta.Id_Agenda, 0, _objParametriServer)

            Dim idMovRaccolta = detRaccolta.Id_Mov
            Dim idMovDetRaccolta = detRaccolta.Id_Mov_Det

            Dim movimentoRaccolta As Movimento = opAgRacc.Movimenti.First(Function(mov) mov.Id_Mov = idMovRaccolta)
            Dim dataMovimento As Date = movimentoRaccolta.Data
            Dim movDettaglioRaccolta As Movimento_Dettaglio = movimentoRaccolta.Movimenti_Dettagli.First(Function(det) det.Id_Mov_Det = idMovDetRaccolta)

            Dim objMovimentoC = CreaOggettoMovimentoCaricoRaccolta(opAgRacc.Piva, detRaccolta.Sa_Cod, opAgRacc.Id_Agenda, dataMovimento, opAgRacc.Lav_Cod, movDettaglioRaccolta.Mat_Cod, movDettaglioRaccolta.Qta, dettaglioConf)
            objMovDettagliC = objMovimentoC.Movimenti_Dettagli(0)

            'Aggancio movimento carico ad agenda
            opAgRacc.Movimenti.Add(objMovimentoC)

            'Aggiorno il mat_cod ed il lotto anche sulla riga di dettaglio di raccolta

            'Selezionando una raccolta, viene impostata sul conferimento la relativa udm, non è però bloccata,
            'quindi in teoria potrebbe essere diversa, anche se normalmente l'utente non dovrebbe avere necessità di cambiarla,
            'in ogni caso sovrascrivo la udm del dettaglio 2200 con quella del conferimento perché questa è coerente con la relativa qta
            'e la uso anche nel dettaglio 7300
            Dim helperMovDet As New Agenda_Movimenti_Dettagli_Helper()
            helperMovDet.ModificaPuntuale(
                opAgRacc.Piva,
                opAgRacc.Sa_Cod,
                opAgRacc.Id_Agenda,
                movDettaglioRaccolta.Id_Mov,
                movDettaglioRaccolta.Id_Mov_Det,
                _objParametriServer,
                matCod:=objMovDettagliC.Mat_Cod,
                udmCod:=dettaglioConf.DB_Udm_Cod,
                lotto:=objMovDettagliC.Lotto)

            Dim helperMovimenti As New Agenda_Movimenti_Helper()
            'Scrivo effettivamente il movimento di carico
            helperMovimenti.Scrivi(objMovimentoC, _objParametriServer)
            'Internamente alla Scrivi viene scritto anche il mov. dettaglio con questo codice:
            'If Not IsNothing(Movimento.Movimenti_Dettagli) AndAlso Movimento.Movimenti_Dettagli.Count > 0 Then

            '    For i = 0 To Movimento.Movimenti_Dettagli.Count - 1

            '        Movimento.Movimenti_Dettagli(i).Id_Agenda = Movimento.Id_Agenda
            '        Movimento.Movimenti_Dettagli(i).Id_Mov = idMov

            '        Dim objMovDet As New Agenda_Movimenti_Dettagli_Helper
            '        objMovDet.Scrivi(Movimento.Movimenti_Dettagli(i), objParametri, flagScriviSempreLotto, documentoPrevisionale)
            '        objMovDet = Nothing

            '    Next

            'End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return objMovDettagliC

    End Function

    Public Function ScriviScaricoRaccolteAziendaAgricola(ByVal operazioneSvoltaSulConferimento As enum_TipoOperazioneDB, ByVal conferimentoPiva As String,
                                                         ByVal conferimentoIdAgenda As Integer, ByVal conferimentoIdMov As Integer, ByVal conferimentoIdMovDet As Integer,
                                                         ByVal recuperaBollaEmessaIdAgenda As Integer,
                                                         Optional ByVal dettagliCaricoRaccolte As List(Of Movimento_Dettaglio) = Nothing) As Contabilita_Output

        Const nomeRoutine = "ContabilitaHelper_Dettaglio.ScriviScaricoRaccoltaAziendaAgricola()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim xRisp As Boolean = True
        Dim contabOutputTestata As New Contabilita_Output() With {
            .Risultato = True
        }

        Dim objImpreseImpostazioni As New Imprese_Impostazioni_R
        Dim setupGeneraScaricoAzAgricola As Boolean = True 'Attiva di default

        Try
            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            Dim helperOpAgenda As New Agenda_Operazione_Helper()
            Dim opAgConferimento As Operazione_Agenda = helperOpAgenda.Leggi(conferimentoPiva, 0, conferimentoIdAgenda, 0, _objParametriServer)

            If opAgConferimento IsNot Nothing Then

                Dim movimentoCaricoConf = opAgConferimento.Movimenti.Where(Function(mov) mov.Cau_Mov = CAU_CARICO).First()

                If movimentoCaricoConf.Movimenti_Dettagli.Count > 0 Then

                    Dim dettagliRifCaricoConf As New List(Of Movimento_Dettaglio_Riferimento)()
                    movimentoCaricoConf.Movimenti_Dettagli.ForEach(Function(det)
                                                                       dettagliRifCaricoConf.AddRange(det.Movimenti_Dettagli_Riferimenti)
                                                                       Return True
                                                                   End Function)


                    Dim dettagliRifCaricoConf_BollaEmessa = dettagliRifCaricoConf.Where(Function(detRif) _lavCodScaricoRaccolta.Contains(detRif.Lav_Cod_Rif) AndAlso detRif.Tipo_Associazione = 2)

                    If dettagliRifCaricoConf_BollaEmessa.Count > 0 Then
                        'Ci sono delle righe del conferimento ancora collegate al ddt

                        Dim bollaEmessaPiva = dettagliRifCaricoConf_BollaEmessa.First().Piva_Rif
                        Dim bollaEmessaIdAgenda = dettagliRifCaricoConf_BollaEmessa.First().Id_Agenda_Rif

                        Dim opAgBollaEmessa As Operazione_Agenda = helperOpAgenda.Leggi(bollaEmessaPiva, 0, bollaEmessaIdAgenda, 0, _objParametriServer)

                        If opAgBollaEmessa IsNot Nothing AndAlso operazioneSvoltaSulConferimento = enum_TipoOperazioneDB.Cancellazione Then
                            'In caso ho cancellato una riga ma il ddt emesso è ancora presente a db significa che la riga non era collegata allo stesso e di
                            'conseguenza questo non è da modificare
                        ElseIf opAgBollaEmessa IsNot Nothing Then
                            'Sono in modifica o scrittura di una riga che non ha cancellato il ddt, di conseguenza
                            '- o non è collegata allo stesso ma potrei doverla collegare se il parametro dettagliCaricoRaccolte è diverso da nothing
                            If dettagliCaricoRaccolte IsNot Nothing Then
                                'Scrivo questi nuovi dettagli
                                Dim bollaEmessaIdMovScarico = opAgBollaEmessa.Movimenti.Where(Function(mov) mov.Cau_Mov = CAU_SCARICO).First().Id_Mov

                                Dim dettagliCaricoConferimento As New List(Of Movimento_Dettaglio)()

                                If movimentoCaricoConf.Movimenti_Dettagli.Count > 1 Then
                                    dettagliCaricoConferimento = New List(Of Movimento_Dettaglio) From {movimentoCaricoConf.Movimenti_Dettagli.Where(Function(det) det.Id_Mov_Det = conferimentoIdMovDet).First()}
                                End If

                                ScriviDettagliScaricoRaccolteAziendaAgricola(bollaEmessaIdAgenda, bollaEmessaIdMovScarico, dettagliCaricoRaccolte, conferimentoPiva, movimentoCaricoConf.Data, dettagliCaricoConferimento)
                            End If
                        Else 'If opAgBollaEmessa Is Nothing Then
                            'Ho modificato o cancellato un dettaglio che era collegato al ddt ma ce ne sono ancora altri che mantengono il collegamento,
                            'quindi devo riscrivere il ddt ri-elaborando tali dettagli
                            'ed eventualmente inserendo i dettagli riferiti alla riga corrente dati dal parametro dettagliCaricoRaccolte

                            Dim ScaricoSemplice As Boolean = False

                            If opAgConferimento.Piva = bollaEmessaPiva Then
                                ScaricoSemplice = True
                            End If

                            contabOutputTestata = ScriviTestataCaricoScaricoAziendaAgricola(opAgConferimento, bollaEmessaPiva, bollaEmessaIdAgenda, ScaricoSemplice, CAU_SCARICO)

                            If contabOutputTestata.Risultato = True Then

                                Dim dettagliRifCaricoConf_Raccolte = dettagliRifCaricoConf.Where(Function(detRif) detRif.Id_Mov_Det <> conferimentoIdMovDet AndAlso detRif.Lav_Cod_Rif = LAVCOD_RACCOLTA)

                                Dim dettagliCaricoRaccolte_CollegateAltreRigheConf As New List(Of Movimento_Dettaglio)()

                                For Each detRif As Movimento_Dettaglio_Riferimento In dettagliRifCaricoConf_Raccolte

                                    Dim opAgRaccolta = helperOpAgenda.Leggi(detRif.Piva_Rif, detRif.Sa_Cod_Rif, detRif.Id_Agenda_Rif, detRif.Lav_Cod_Rif, _objParametriServer)

                                    Dim movimentoCaricoRaccolta = opAgRaccolta.Movimenti.Where(Function(mov) mov.Cau_Mov = CAU_CARICO)

                                    If movimentoCaricoRaccolta.Count > 0 Then
                                        dettagliCaricoRaccolte_CollegateAltreRigheConf.AddRange(
                                            movimentoCaricoRaccolta.First().Movimenti_Dettagli.Where(Function(det) detRif.Id_Mov_Det_Rif = det.Id_Mov_Det)
                                        )
                                    End If
                                Next

                                Dim dettagliCaricoConferimento As New List(Of Movimento_Dettaglio)()
                                If dettagliCaricoRaccolte IsNot Nothing Then
                                    'Potrei non avere dei dettagli da aggiungere, ma solo quelli da ripristinare, in quest'ultimo caso non devo scrivere righe di
                                    'mov_dettagli_riferimenti perché già presenti
                                    dettagliCaricoConferimento.Add(movimentoCaricoConf.Movimenti_Dettagli.Where(Function(det) det.Id_Mov_Det = conferimentoIdMovDet).First())

                                    dettagliCaricoRaccolte_CollegateAltreRigheConf.AddRange(dettagliCaricoRaccolte)

                                End If

                                ScriviDettagliScaricoRaccolteAziendaAgricola(bollaEmessaIdAgenda, contabOutputTestata.Id_Mov_Scarico, dettagliCaricoRaccolte_CollegateAltreRigheConf, conferimentoPiva, movimentoCaricoConf.Data, dettagliCaricoConferimento)

                            End If

                        End If
                    Else
                        'Ho inserito la prima riga da collegare al ddt da creare oppure
                        'ho modificato l'unica che era collegata
                        'In entrambi i casi devo scrivere il ddt da capo ed inserire i dettagli dati dal parametro dettagliCaricoRaccolte
                        'Non ho bisogno di leggere la mov_dettagli_riferimenti per le raccolte, in quanto sarebbero le stesse che ho in dettagliCaricoRaccolte
                        If dettagliCaricoRaccolte IsNot Nothing Then

                            'Leggo il setup "Crea scarico su azienda agricola" per la piva dell'impresa ma non i centri aziendali
                            Dim strSetupGeneraScaricoAzAgricola = objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser(
                                dettagliCaricoRaccolte.First().Piva, New List(Of Integer)({0}), enum_Impostazioni_Utenti.Scarico_Da_Raccolta,
                                "", _objParametriUtenti, _objParametriServer)

                            If strSetupGeneraScaricoAzAgricola = "0" Then
                                setupGeneraScaricoAzAgricola = False
                            End If

                            'Devo generare lo scarico se:
                            '1) il setup è abilitato oppure
                            '2) sono sulla stessa azienda (in quanto devo contro-bilanciare il carico che a questo punto dell'esecuzione è sempre salvato) oppure
                            '3) sono in modifica di una riga che era collegata al ddt
                            If setupGeneraScaricoAzAgricola = True OrElse
                                dettagliCaricoRaccolte.First().Piva = conferimentoPiva OrElse
                                (operazioneSvoltaSulConferimento = enum_TipoOperazioneDB.Modifica AndAlso recuperaBollaEmessaIdAgenda <> 0) Then

                                Dim ScaricoSemplice As Boolean = False

                                If opAgConferimento.Piva = dettagliCaricoRaccolte.First().Piva Then
                                    ScaricoSemplice = True
                                End If

                                contabOutputTestata = ScriviTestataCaricoScaricoAziendaAgricola(opAgConferimento, dettagliCaricoRaccolte.First().Piva, recuperaBollaEmessaIdAgenda, ScaricoSemplice, CAU_SCARICO)
                                If contabOutputTestata.Risultato = True Then
                                    Dim dettagliCaricoConferimento As New List(Of Movimento_Dettaglio) From {movimentoCaricoConf.Movimenti_Dettagli.Where(Function(det) det.Id_Mov_Det = conferimentoIdMovDet).First()}
                                    ScriviDettagliScaricoRaccolteAziendaAgricola(contabOutputTestata.Id_Agenda, contabOutputTestata.Id_Mov_Scarico, dettagliCaricoRaccolte, conferimentoPiva, movimentoCaricoConf.Data, dettagliCaricoConferimento)
                                End If
                            End If
                        End If

                    End If

                End If

            End If

            If contabOutputTestata.Risultato = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return contabOutputTestata
    End Function

    Public Function ScriviTestataCaricoScaricoAziendaAgricola(ByVal OperazioneAgenda As Operazione_Agenda, ByVal conferentePiva As String, ByVal bollaEmessaIdAgenda As Integer, ByVal CaricoScaricoSemplice As Boolean, ByVal CAU_MOV As String, Optional ByVal Blocco_Flag As Integer = 0, Optional ByVal DescrizioneAggiuntiva As String = "") As Contabilita_Output

        Const nomeRoutine = "ContabilitaHelper_Dettaglio.ScriviTestataCaricoScaricoAziendaAgricola()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim contabOutput As New Contabilita_Output() With {
            .Risultato = True
        }
        Dim handleImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read


        Try
            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            Dim cessionarioPiva = OperazioneAgenda.Piva

            Dim cessionarioRisUm As Integer
            Dim cessionarioRagSoc As String = ""

            If Not CaricoScaricoSemplice Then

                'Leggo i contatti della azienda conferente ed i contatti pubblici perché devo recuperare il cod_risum per l'azienda che riceve il prodotto
                Dim handleRisorseUmane As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R

                Dim xfiltroAggiuntivo As String = String.Format("Risorse_Umane.Cod_Contatto = '{0}'", cessionarioPiva)
                Dim dtRisUmConferimento As DataTable = handleRisorseUmane.LeggiRapportoSpecifico(conferentePiva, xfiltroAggiuntivo, "", _objParametriServer)

                If dtRisUmConferimento.Rows.Count = 0 Then
                    contabOutput.Risultato = False

                    Dim conferenteRagSoc = handleImprese.RagSoc_from_Piva(conferentePiva, _objParametriServer)
                    cessionarioRagSoc = handleImprese.RagSoc_from_Piva(cessionarioPiva, _objParametriServer)

                    contabOutput.MsgError = String.Format(
                        "Non è possibile registrare il documento di trasporto emesso nell'azienda {0} (conferente/produttore) " &
                        "perché non possiede fra i propri contatti l'azienda {1} (cessionaria del conferimento)", conferenteRagSoc, cessionarioRagSoc)

                ElseIf dtRisUmConferimento.Rows.Count = 1 Then
                    'Se l'azienda è registrata solo una volta, la scelgo senza verificarne il rapporto contabile
                    cessionarioRisUm = dtRisUmConferimento(0)("Cod_RisUm")
                    cessionarioRagSoc = dtRisUmConferimento(0)("Rag_Soc")

                Else
                    'Altrimenti cerco se ho l'azienda registrata con il rapporto "Cliente"
                    Dim arrRisUmConferimento = dtRisUmConferimento.Select("Cod_Rapporto = " & COD_CLIENTE)

                    If arrRisUmConferimento.Length = 1 Then
                        cessionarioRisUm = arrRisUmConferimento(0)("Cod_RisUm")
                        cessionarioRagSoc = arrRisUmConferimento(0)("Rag_Soc")
                    Else
                        arrRisUmConferimento = dtRisUmConferimento.Select("Cliente = 1")

                        If arrRisUmConferimento.Length = 0 Then
                            contabOutput.Risultato = False

                            Dim conferenteRagSoc = handleImprese.RagSoc_from_Piva(conferentePiva, _objParametriServer)
                            cessionarioRagSoc = handleImprese.RagSoc_from_Piva(cessionarioPiva, _objParametriServer)

                            contabOutput.MsgError = String.Format(
                                "Non è possibile registrare il documento di trasporto emesso nell'azienda {0} (conferente/produttore) " &
                                "perché non possiede l'azienda {1} (cessionaria del conferimento) con un rapporto contabile di clientela", conferenteRagSoc, cessionarioRagSoc)

                        Else
                            cessionarioRisUm = arrRisUmConferimento(0)("Cod_RisUm")
                            cessionarioRagSoc = arrRisUmConferimento(0)("Rag_Soc")
                        End If

                    End If

                End If

            End If

            If contabOutput.Risultato = True Then
                Dim cessionarioCodIndirizzo As Integer = 0

                If Not CaricoScaricoSemplice Then
                    Dim objInd As New AgronicaCoreAnagrafeDAL.Indirizzi_Read
                    Dim dtIndirizzi As DataTable = objInd.LeggiIndContattiImpreseCentri(conferentePiva, cessionarioPiva, 0, 0, "", "", _objParametriServer)

                    If dtIndirizzi.Rows.Count > 0 Then
                        Dim arrIndSedeOp = dtIndirizzi.Select("Tipo_Indirizzo = " & INDIRIZZO_SEDE_OPERATIVA)

                        cessionarioCodIndirizzo = arrIndSedeOp(0)("Cod_Indirizzo")
                    End If
                End If

                Dim impreseImpostazioniR As New Imprese_Impostazioni_R

                Dim valImpLayoutPeso, valImpLayoutPrezzo, valImpLayoutRiscontrato As Integer

                valImpLayoutPeso = impreseImpostazioniR.LeggiScalareMulticentroAziendaSuperUser(conferentePiva, Nothing,
                                                                                  enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT,
                                                                                  0,
                                                                                  _objParametriUtenti,
                                                                                  _objParametriServer)

                valImpLayoutPrezzo = impreseImpostazioniR.LeggiScalareMulticentroAziendaSuperUser(conferentePiva, Nothing,
                                                                                  enum_Impostazioni_Utenti.SuperUser_LayOut_Prezzo_DDT,
                                                                                  0,
                                                                                  _objParametriUtenti,
                                                                                  _objParametriServer)

                valImpLayoutRiscontrato = impreseImpostazioniR.LeggiScalareMulticentroAziendaSuperUser(conferentePiva, Nothing,
                                                                                  enum_Impostazioni_Utenti.SuperUser_LayOut_Riscontrato_DDT,
                                                                                  0,
                                                                                  _objParametriUtenti,
                                                                                  _objParametriServer)

                Dim impFormatiStampa = AgronicaCoreContabHLP.Contabilita.GetLayoutFormatiStampa(valImpLayoutPeso,
                                                                              valImpLayoutPrezzo,
                                                                              valImpLayoutRiscontrato)

                Dim UsernameModifica As String = OperazioneAgenda.Username_Modifica
                Dim DataMovimento As New Date
                Dim DocNumeroSin As String
                Dim DocNumero As Integer
                Dim DocNumeroDes As String
                Dim DataRegistrazione As New Date
                Dim OraSpedizione As New Date
                Dim Aspetto As String
                Dim Data As New Date
                Dim Blocco_Data As Date = AGRODATAINIZIO
                Dim Blocco_Username As String = String.Empty

                'Controllo se provengo dal giro dei documenti contabili
                Dim conferimentoMovimentiRegistrazioneSecondario = OperazioneAgenda.Movimenti.Where(Function(mov) mov.Cau_Mov = CAU_REGISTRAZIONE_SECONDARIA)

                If Not IsNothing(conferimentoMovimentiRegistrazioneSecondario) AndAlso conferimentoMovimentiRegistrazioneSecondario.Count > 0 Then

                    Dim conferimentoMovRegistrazioneSecondario = conferimentoMovimentiRegistrazioneSecondario.First()

                    DataMovimento = conferimentoMovRegistrazioneSecondario.Data
                    DocNumeroSin = If(Not CaricoScaricoSemplice, conferimentoMovRegistrazioneSecondario.Doc_Numero_Sin, "")
                    DocNumero = If(Not CaricoScaricoSemplice, conferimentoMovRegistrazioneSecondario.Doc_Numero, 0)
                    DocNumeroDes = If(Not CaricoScaricoSemplice, conferimentoMovRegistrazioneSecondario.Doc_Numero_Des, "")
                    DataRegistrazione = conferimentoMovRegistrazioneSecondario.Data_Registrazione
                    OraSpedizione = If(Not CaricoScaricoSemplice, conferimentoMovRegistrazioneSecondario.Data, AGRODATAFINE)
                    Aspetto = If(Not CaricoScaricoSemplice, conferimentoMovRegistrazioneSecondario.Aspetto, "")
                    Data = conferimentoMovRegistrazioneSecondario.Data
                Else
                    DataMovimento = OperazioneAgenda.Data
                    DocNumeroSin = ""
                    DocNumero = 0
                    DocNumeroDes = ""
                    DataRegistrazione = OperazioneAgenda.Data
                    OraSpedizione = If(Not CaricoScaricoSemplice, OperazioneAgenda.Data, AGRODATAFINE)
                    Aspetto = If(Not CaricoScaricoSemplice, "VISIBILE", "")
                    Data = OperazioneAgenda.Data
                End If

                If Blocco_Flag = 1 Then
                    Blocco_Data = Data
                    Blocco_Username = _objParametriServer.UsernameOperazione
                End If

                Dim CausaleTrasportoCod As Integer = 0

                Dim CausaleTrasporto As String = ""

                Dim lavCodDaAssegnare As Integer = 0

                If CAU_MOV = CAU_CARICO Then
                    lavCodDaAssegnare = LAVCOD_CARICO

                    If Not CaricoScaricoSemplice Then
                        lavCodDaAssegnare = LAVCOD_BOLLA_RICEVUTA

                        CausaleTrasportoCod = enum_CausaliTrasporto.ACQUISTO

                        CausaleTrasporto = "ACQUISTO"
                    End If
                ElseIf CAU_MOV = CAU_SCARICO Then
                    lavCodDaAssegnare = LAVCOD_SCARICO

                    If Not CaricoScaricoSemplice Then
                        lavCodDaAssegnare = LAVCOD_BOLLA_EMESSA

                        CausaleTrasportoCod = enum_CausaliTrasporto.VENDITA

                        CausaleTrasporto = "VENDITA"
                    End If
                End If

                Dim opAgScarico As New Contabilita_Testata() With {
                    .Piva = conferentePiva,
                    .SaCod = 0,
                    .IdAgenda = bollaEmessaIdAgenda,
                    .IdMov = 0,
                    .LavCod = lavCodDaAssegnare,
                    .DataMovimento = DataMovimento,
                    .DocNumeroSin = DocNumeroSin,
                    .DocNumero = DocNumero,
                    .DocNumeroDes = DocNumeroDes,
                    .UsernameModifica = UsernameModifica,
                    .CodRisUm = cessionarioRisUm,
                    .RagSoc = cessionarioRagSoc,
                    .CodIndirizzoRisUm = cessionarioCodIndirizzo,
                    .DataRegistrazione = DataRegistrazione,
                    .ProgrRegistrazione = 0,
                    .DocNumeroCarattereFormattazione = "",
                    .DocNumeroLunghezza = 0,
                    .OraSpedizione = OraSpedizione,
                    .Layout_FormatiStampa = impFormatiStampa,
                    .Aspetto = Aspetto,
                    .CausaleTrasportoCod = CausaleTrasportoCod,
                    .CausaleTrasporto = CausaleTrasporto,
                    .BloccoFlag = Blocco_Flag,
                    .BloccoData = Blocco_Data,
                    .BloccoUsername = Blocco_Username,
                    .DescrizioneAggiuntiva = DescrizioneAggiuntiva
                }
                '.MovimentoCarico = MovimentoCarico,
                '.MovimentoScarico = MovimentoScarico

                Dim objContabHelper As New ContabilitaHelper_Testata(_objParametriServer, _objParametriUtenti)
                Try
                    contabOutput = objContabHelper.ScriviTestataDocumento(opAgScarico, True)
                Catch ex As Exception
                    Throw New GiasException("Errore in scrittura testata ddt emesso per raccolte: " & ex.Message)
                End Try

                If contabOutput.Risultato = True Then

                    If Not CaricoScaricoSemplice Then
                        'MOVIMENTO DI SCARICO/CARICO
                        Dim objMovimentoScarico = New Movimento With {
                            .Piva = conferentePiva,
                            .Sa_Cod = 0,
                            .Id_Agenda = contabOutput.Id_Agenda,
                            .Id_Mov = 0,
                            .Lav_Cod = lavCodDaAssegnare,
                            .Cau_Mov = CAU_MOV,
                            .Data = Data,
                            .Scadenza = AGRODATAFINE,
                            .Doc_Numero_Sin = DocNumeroSin,
                            .Doc_Numero = contabOutput.Doc_Numero, 'DocNumero,
                            .Doc_Numero_Des = DocNumeroDes,
                            .Cod_Risum = cessionarioRisUm,
                            .Cod_IndirizzoRisUm = cessionarioCodIndirizzo,
                            .Cod_Destinazione = 0,
                            .Cod_IndirizzoDestinazione = 0,
                            .Cod_Vettore = 0,
                            .Cod_IndirizzoVettore = 0,
                            .Cod_RisUm_Aggiuntivo = 0,
                            .Cod_Indirizzo_Aggiuntivo = 0,
                            .Extra_Date = #12/30/1899#,
                            .Scadenza_Extra = AGRODATAINIZIO
                        }

                        Dim objSequenze As New Agro_Sequenze
                        objMovimentoScarico.Id_Mov = objSequenze.NuovoId_Tabella("Movimenti", 0, 200000000, _objParametriServer)

                        contabOutput.Id_Mov_Scarico = objMovimentoScarico.Id_Mov

                        Dim objMovHelper As New Agenda_Movimenti_Helper
                        Try

                            Dim dummy = objMovHelper.Scrivi(objMovimentoScarico, _objParametriServer, flagUsaOraReale:=True)

                        Catch ex As Exception
                            Throw New GiasException("Errore in scrittura movimento di scarico ddt emesso per raccolte: " & ex.Message)
                        End Try

                    Else
                        contabOutput.Id_Mov_Scarico = contabOutput.Id_Mov_Testata
                    End If

                End If

            End If


            If contabOutput.Risultato = False Then
                Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

        Return contabOutput
    End Function

    Private Sub ScriviDettagliScaricoRaccolteAziendaAgricola(ByVal bollaEmessaIdAgenda As Integer, ByVal bollaEmessaIdMov As Integer, ByVal dettagliCaricoRaccolte As List(Of Movimento_Dettaglio), ByVal cessionarioPiva As String, ByVal dataMovimento As Date, ByVal dettagliCaricoConferimento As List(Of Movimento_Dettaglio))

        Const nomeRoutine = "ContabilitaHelper_Dettaglio.ScriviDettagliScaricoRaccolteAziendaAgricola()"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim helperDettagli As New Agenda_Movimenti_Dettagli_Helper()
        Dim helperDettagliRiferimenti As New Agenda_Movimenti_Dettagli_Riferimenti_Helper()
        Dim objSequenze As New Agro_Sequenze()

        Dim ordineDet As Integer = 1

        Dim detLog As New Movimento_Dettaglio()

        Try
            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            If dettagliCaricoRaccolte.Count > 0 Then

                Dim autoConferimento As Boolean = cessionarioPiva = dettagliCaricoRaccolte.First().Piva
                Dim lavCod As Integer = If(Not autoConferimento, LAVCOD_BOLLA_EMESSA, LAVCOD_SCARICO)

                For Each det As Movimento_Dettaglio In dettagliCaricoRaccolte


                    'Per scrivere i record di dettaglio della bolla di scarico prendo come base i record di dettaglio del carico delle raccolte relative e ne cambio i dati fondamentali
                    det.Id_Agenda = bollaEmessaIdAgenda
                    det.Id_Mov = bollaEmessaIdMov
                    det.Id_Mov_Det = objSequenze.NuovoId_Tabella("Movimenti_Dettagli", 0, 200000000, _objParametriServer)
                    det.Movimenti_Destinazioni.First().Id_Mov_Det = det.Id_Mov_Det
                    det.Data_Creazione = Now
                    det.Data_Modifica = Now
                    det.Anno = If(Not autoConferimento, dataMovimento.Year, 0)

                    If det.Mat_Cod > 0 AndAlso det.Pro_Cod = 0 Then

                        Dim handleMatPrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R()
                        det.Mov_Det_Des = handleMatPrime.MatDes_from_MatCod(cessionarioPiva, TRASFORMATI_VEGETALI, det.Mat_Cod, "", "", "", _objParametriServer)

                    ElseIf det.Mat_Cod = 0 AndAlso det.Pro_Cod > 0 Then

                        Dim handleFormulati As New AgronicaCoreMetaSchemaDAL.Formulati_R
                        det.Mov_Det_Des = handleFormulati.FrDes_from_FrCod(det.Pro_Cod, _objParametriServer) + " (" + det.Pro_Cod + ")"

                    End If

                    If det.Udm_Cod = enum_UnitaMisura.KG Then
                        'Allineo la scrittura della qta come in GeneraRigaContabilita, necessario per le stampe
                        det.Qta_Extra_Totale = det.Qta
                    End If
                    det.Ordine_Det = If(Not autoConferimento, ordineDet, 0)
                    det.Cod_Conto = If(Not autoConferimento, UtilityHelper.GetConto(lavCod, enumTipoConto.Economico, 0), 0)
                    det.Cod_Conto_Pat = If(Not autoConferimento, UtilityHelper.GetConto(lavCod, enumTipoConto.Patrimoniale, 0), 0)
                    det.Contabilizzato = If(Not autoConferimento, CONTABILE, NONCONTABILE)
                    det.Ric_Cod = If(Not autoConferimento, 2, 0)
                    det.Pendente = If(Not autoConferimento, enum_Pendenza.DocBolla, enum_Pendenza.Alienazione_Pendenza)
                    If autoConferimento Then
                        det.Cod_Iva = 0
                        det.Iva = 0
                        det.ChkIva_Manuale = 0
                    End If
                    ordineDet += 1

                    det.Movimenti_Dettagli_Riferimenti = Nothing

                    detLog = det
                    helperDettagli.Scrivi(det, _objParametriServer)

                Next


                'Creo i collegamenti fra il ddt sull'azienda agricola ed i dettagli del conferimento sul cessionario, avrò un record per ogni dettaglio del conferimento,
                'in quanto ognuno di essi è collegato all'intero ddt
                For Each confDet As Movimento_Dettaglio In dettagliCaricoConferimento
                    Dim detRif As New Movimento_Dettaglio_Riferimento With {
                        .Piva = confDet.Piva,
                        .Sa_Cod = confDet.Sa_Cod,
                        .Id_Agenda = confDet.Id_Agenda,
                        .Id_Mov = confDet.Id_Mov,
                        .Id_Mov_Det = confDet.Id_Mov_Det,
                        .Lav_Cod = confDet.Lav_Cod,
                        .Cau_Mov = CAU_CARICO,
                        .Piva_Rif = dettagliCaricoRaccolte.First().Piva,
                        .Sa_Cod_Rif = 0,
                        .Id_Agenda_Rif = bollaEmessaIdAgenda,
                        .Id_Mov_Rif = -1,
                        .Id_Mov_Det_Rif = -1,
                        .Lav_Cod_Rif = lavCod,
                        .Cau_Mov_Rif = CAU_SCARICO,
                        .Tipo_Associazione = 2
                    }

                    helperDettagliRiferimenti.Scrivi(detRif, _objParametriServer)
                Next
            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ " & nomeRoutine & " ] : Errore in scrittura dettaglio '" & detLog.Mov_Det_Des & "' ddt emesso per raccolte: " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)
        End Try

    End Sub

End Class
