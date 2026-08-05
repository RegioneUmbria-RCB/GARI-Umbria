Imports System.Text
Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreModello
Imports AgronicaCoreModello.Utility_Agenda
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.dettagli
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.metaschema.avversita
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.exceptions

Public Class CheckAttivita

    Enum TipoErrore
        Bloccante
        BloccantePerImpostazioneUtente
        NonBloccante
        NonBloccantePerImpostazioneUtente
    End Enum

    Public Function ControlloInserimentoDoseProdotto(input As Controllo_Inserimento_Dose_Prodotto, objParametri_Super_Server As AgronicaCoreParametri,
                                                     objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As List(Of ErroreGias)

        Dim listaErrori As New List(Of ErroreGias)

        'DT: nel caso di lotto facoltativo lasciato vuoto dall'utente, si prende comunque "" per la verifica delle giacenze, perchè comunque non bisogna sommare tutti i movimenti
        '(quelli con lotto valorizzato sono considerati diversi da quelli senza lotto, non vanno sommati)
        'Per questo motivo passiamo "" e non "NON DEFINITO"
        input.Lotto = If(input.Lotto IsNot Nothing, input.Lotto, "")

        'GRIGLIA SENZA RIGHE IN MODIFICA/ELIMINAZIONE, RAGGRUPPATE PER PRODOTTO/MAGAZZINO/LOTTO
        Dim gridDosiManaged = getGridDosiManaged(input)

        Dim Lav_Cod = input.lavorazione.primaryKey.codice
        Dim InfoOperazione As InfoOperazione = GetInfoOperazione(Lav_Cod, tipoAttivita:=1) 'RR: Ho fissato tipoAttivita a QDC perchè non si controlla serve sapere solo operazione("Trattamento..")

        If InfoOperazione.IsTrattamento = False And InfoOperazione.IsFertilizzazione = False And InfoOperazione.IsSemina = False Then
            Return listaErrori
        End If

        'OBBLIGATORIETA' --> WARNING BLOCCANTI CON EXIT IMMEDIATO
        CheckObbligatorieta(input, InfoOperazione, input.parametri_aggiuntivi_list, objParametri_Server, objParametri_Utenti, listaErrori)
        If listaErrori.Count > 0 Then
            Return listaErrori
        End If

        'COMPATIBILITA' FRA PRODOTTO CORRENTE E PRODOTTI GIA' SELEZIONATI (IN GRIGLIA) --> WARNING BLOCCANTI CON EXIT IMMEDIATO
        CheckCompatibilita(input, gridDosiManaged, InfoOperazione, listaErrori)
        If listaErrori.Count > 0 Then
            Return listaErrori
        End If

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Blocca_Inserimento = objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_NONCONFORME, objParametri_Utenti.UtenteUsername, objParametri_Utenti)

        Dim errori As New Dictionary(Of TipoErrore, List(Of String))
        Dim titolo As String = ""

        Dim hasErroriBloccanti As Boolean = False
        Dim strErroreGlobale As String = ""

        '=======================================
        '   RECUPERO FLAG PER MOSTRARE WARNING
        '---------------------------------------
        Dim mostraWarning_CheckGiacenza, mostraWarning_CheckMassimali, mostraWarning_CheckEtichetta, mostraWarning_CheckProdottoInRibaltamento As Boolean

        If input.parametri_aggiuntivi_list IsNot Nothing Then
            For Each parametroAggiuntivo In input.parametri_aggiuntivi_list
                Select Case parametroAggiuntivo.key
                    Case Key_Parametri_Aggiuntivi_ControlloDosi.mostraWarning_CheckGiacenza
                        mostraWarning_CheckGiacenza = CBool(parametroAggiuntivo.value)
                    Case Key_Parametri_Aggiuntivi_ControlloDosi.mostraWarning_CheckMassimali
                        mostraWarning_CheckMassimali = CBool(parametroAggiuntivo.value)
                    Case Key_Parametri_Aggiuntivi_ControlloDosi.mostraWarning_CheckEtichetta
                        mostraWarning_CheckEtichetta = CBool(parametroAggiuntivo.value)
                    Case Key_Parametri_Aggiuntivi_ControlloDosi.mostraWarning_CheckProdottoInRibaltamento
                        mostraWarning_CheckProdottoInRibaltamento = CBool(parametroAggiuntivo.value)
                End Select
            Next
        End If

#Region "Controllo differenze tra il Prodotto confermato durante il ribaltamento e il Prodotto salvato nelle Ricette/Brogliaccio (WARNING - SOLO DURANTE IL RIBALTAMENTO IN AGENDA)"

        If mostraWarning_CheckProdottoInRibaltamento Then

            ChecKProdottoInRibaltamento(input, InfoOperazione, objParametri_Server, errori)

            titolo = My.Resources.AgronicaCoreMapper.DifferenzeRispettoARicetta

            hasErroriBloccanti = BuildErrorMessage(errori, strErroreGlobale, titolo)

            If Not String.IsNullOrEmpty(strErroreGlobale) Then

                listaErrori.Add(generaErroreGias(ErroreGias_Severity.Warning, "", strErroreGlobale, Key_Parametri_Aggiuntivi_ControlloDosi.mostraWarning_CheckProdottoInRibaltamento))

                Return listaErrori
            Else
                titolo = ""
            End If
        End If

#End Region


#Region "Livello 1 - Giacenza + varie (WARNING / WARNING BLOCCANTI)"

        ClearErrori(errori)

        'GIACENZA MAGAZZINO --> WARNING / WARNING BLOCCANTI
        CheckGiacenzaMagazzino(input, gridDosiManaged, InfoOperazione, errori, objParametri_Utenti, objParametri_Server)

        Dim isErroreBloccante As Boolean = BuildErrorMessage(errori, strErroreGlobale, titolo:="")
        If isErroreBloccante Then
            hasErroriBloccanti = isErroreBloccante
        End If

        If InfoOperazione.IsTrattamento Then

            ClearErrori(errori)

            Dim listaErroriTemp As New List(Of ErroreGias)

            If InfoOperazione.TipoCentroDiCosto = centri_di_costo.Tipo.Esercizio Then
                'CARENZA --> WARNING / WARNING BLOCCANTI
                CheckCarenza(input, listaErroriTemp, objParametri_Utenti)

                'TODO_DT: TESTARE!!!! dursban su pesco Barbieri
                'BLOCCO FIORITURA --> WARNING / WARNING BLOCCANTI
                CheckBloccoFioritura(input, listaErroriTemp, objParametri_Utenti, objParametri_Server, objParametri_Super_Server)

                'BUFFER --> WARNING / WARNING BLOCCANTI
                CheckBuffer(input, listaErroriTemp, objParametri_Utenti)
            End If

            'MISCIBILITA POLVERULENTI E NON --> WARNING / WARNING BLOCCANTI
            CheckMixPolverulento(input, gridDosiManaged, Blocca_Inserimento, listaErroriTemp, objParametri_Utenti)

            If listaErroriTemp.Count > 0 Then
                Select Case Blocca_Inserimento
                    Case "1" 'blocco
                        For Each errore In listaErroriTemp
                            errori(TipoErrore.BloccantePerImpostazioneUtente).Add(errore.messaggio)
                        Next

                    Case "2" 'avviso
                        For Each errore In listaErroriTemp
                            errori(TipoErrore.NonBloccantePerImpostazioneUtente).Add(errore.messaggio)
                        Next

                End Select
            End If

            isErroreBloccante = BuildErrorMessage(errori, strErroreGlobale, titolo:="")
            If isErroreBloccante Then
                hasErroriBloccanti = isErroreBloccante
            End If

        End If

        'PERIODO DIVIETO (DIRETTIVA NITRATI) --> WARNING
        If InfoOperazione.IsFertilizzazione Then

            ClearErrori(errori)

            'Controllo periodo divieto 
            'Lo facciamo sia in agenda sia nella ricetta (ogni tipo di ricetta, non solo quelle di tipo pua),
            'solo nell’operazione distribuzione ammendanti con direttiva nitrati e solo se esiste un pua attivo alla data
            'dell’operazione (se sono più di uno in quella data prendiamo il più recente). 
            If Lav_Cod = LAVCOD_DISTRIBUZIONE_AMMENDANTI AndAlso
                Not IsNothing(input.pua) AndAlso
                input.pua.codice > 0 AndAlso
                input.pua.disciplinare IsNot Nothing AndAlso
                input.pua.disciplinare.regolamentoConcimazione IsNot Nothing AndAlso
                input.pua.disciplinare.regolamentoConcimazione.tipo = enum_PUARegolamenti_Tipo.PUA Then

                CheckPeriodoDivieto(input, errori, objParametri_Server)

                isErroreBloccante = BuildErrorMessage(errori, strErroreGlobale, titolo:="")
                If isErroreBloccante Then
                    hasErroriBloccanti = isErroreBloccante
                End If

            End If
        End If

        If strErroreGlobale <> "" Then
            FormatErroreGlobale(strErroreGlobale)
            If hasErroriBloccanti Then
                listaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", strErroreGlobale, ""))
            Else
                If mostraWarning_CheckGiacenza Then
                    listaErrori.Add(generaErroreGias(ErroreGias_Severity.Warning, "", strErroreGlobale, Key_Parametri_Aggiuntivi_ControlloDosi.mostraWarning_CheckGiacenza))
                End If
            End If
        End If

        If listaErrori.Count > 0 Then
            Return listaErrori
        End If

#End Region

#Region "Livello 2 - Massimali fertilizzazione (WARNING / WARNING BLOCCANTI)"

        'TODO Chiedere se va bene così
        If (InfoOperazione.IsFertilizzazione OrElse InfoOperazione.IsTrattamento) AndAlso
            InfoOperazione.Elem_Cod <> INSETTI AndAlso
            CInt(Lav_Cod) <> LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE AndAlso
            CInt(Lav_Cod) <> LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA AndAlso
            CInt(Lav_Cod) <> LAVCOD_REINNESCO_TRAPPOLE AndAlso
            InfoOperazione.TipoCentroDiCosto = centri_di_costo.Tipo.Esercizio Then

            ClearErrori(errori)

            hasErroriBloccanti = False
            strErroreGlobale = ""

            Dim Qta_Ha_Distribuibile_Prodotto As Decimal = 0
            Dim Qta_Ha_Distribuibile_Prodotto_Ricette As Decimal = 0
            CheckMassimali(ConsideraProdottoInInserimento:=True, input, gridDosiManaged, Utility.GetVegCodFromUtilizzoTerreno(input.utilizzoTerreno), Qta_Ha_Distribuibile_Prodotto, Qta_Ha_Distribuibile_Prodotto_Ricette, errori, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            hasErroriBloccanti = BuildErrorMessage(errori, strErroreGlobale, titolo:="")

            If strErroreGlobale <> "" Then
                FormatErroreGlobale(strErroreGlobale)
                If hasErroriBloccanti Then
                    listaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", strErroreGlobale, ""))
                Else
                    If mostraWarning_CheckMassimali Then
                        listaErrori.Add(generaErroreGias(ErroreGias_Severity.Warning, "", strErroreGlobale, Key_Parametri_Aggiuntivi_ControlloDosi.mostraWarning_CheckMassimali))
                    End If
                End If
            End If

            If listaErrori.Count > 0 Then
                Return listaErrori
            End If

        End If

#End Region

#Region "Livello 3 - Etichetta (WARNING / WARNING BLOCCANTI)"

        If InfoOperazione.IsTrattamento Then

            If input.dosi_Etichetta IsNot Nothing AndAlso input.dosi_Etichetta.Count > 0 Then

                ClearErrori(errori)

                hasErroriBloccanti = False
                strErroreGlobale = ""

                'DT: input.dosi_Etichetta(0) corrisponde alla dose effettivamente selezionata, se ce ne sono altre vengono accodate lato Angular
                Dim doseSelezionata = input.dosi_Etichetta(0)

                'Controlli ETICHETTA (dosi min/max + acqua + numero Trattamenti + intervallo trattamenti)
                CheckDosaggio(doseSelezionata, input, gridDosiManaged, Blocca_Inserimento, InfoOperazione, errori, objParametri_Utenti, objParametri_Server)

                titolo = String.Format(My.Resources.AgronicaCoreMapper.ControllaDoseRispettata, doseSelezionata.DescrizioneConcatenata)
                isErroreBloccante = BuildErrorMessage(errori, strErroreGlobale, titolo)
                If isErroreBloccante Then
                    hasErroriBloccanti = isErroreBloccante
                End If

                'scorro i dosaggi per controllare gli altri nella stessa epoca
                For i = 1 To input.dosi_Etichetta.Count - 1

                    Dim doseDaConfrontare = input.dosi_Etichetta(i)

                    Dim Udm_Cod_Scomposta As Integer = 0
                    Dim HaHl As Integer = 0

                    Dim objUdm As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                    objUdm.ScomponiUdm(Udm_Cod_Scomposta, HaHl, If(doseDaConfrontare.Udm IsNot Nothing, doseDaConfrontare.Udm.codice, 0))

                    '(15/01/2021 fede) introdotta verifica dei dosaggi tra loro solo per i dosaggi dello stesso decreto ( = FormulatiXAllegatiNormative_IDRiga)
                    If doseSelezionata.codice <> doseDaConfrontare.codice AndAlso
                            If(doseSelezionata.Udm IsNot Nothing, doseSelezionata.Udm.codice, 0) <> If(doseDaConfrontare.Udm IsNot Nothing, doseDaConfrontare.Udm.codice, 0) AndAlso
                            (doseSelezionata.FormulatiXAllegatiNormative_IDRiga = doseDaConfrontare.FormulatiXAllegatiNormative_IDRiga OrElse doseDaConfrontare.FormulatiXAllegatiNormative_IDRiga = 0) AndAlso
                                (doseSelezionata.Da_Epoca = doseDaConfrontare.Da_Epoca OrElse doseDaConfrontare.Da_Epoca = 0) AndAlso
                                (doseSelezionata.A_Epoca = doseDaConfrontare.A_Epoca OrElse doseDaConfrontare.A_Epoca = 0) AndAlso
                                (HaHl = 2123) Then

                        ClearErrori(errori)

                        CheckDosaggio(doseDaConfrontare, input, gridDosiManaged, Blocca_Inserimento, InfoOperazione, errori, objParametri_Utenti, objParametri_Server)

                        titolo = String.Format(My.Resources.AgronicaCoreMapper.ControllaDoseRispettata, doseDaConfrontare.DescrizioneConcatenata)
                        isErroreBloccante = BuildErrorMessage(errori, strErroreGlobale, titolo)
                        If isErroreBloccante Then
                            hasErroriBloccanti = isErroreBloccante
                        End If

                    End If
                Next

                If strErroreGlobale <> "" Then
                    FormatErroreGlobale(strErroreGlobale)
                    If hasErroriBloccanti Then
                        listaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", strErroreGlobale, ""))
                    Else
                        If mostraWarning_CheckEtichetta Then
                            listaErrori.Add(generaErroreGias(ErroreGias_Severity.Warning, "", strErroreGlobale, Key_Parametri_Aggiuntivi_ControlloDosi.mostraWarning_CheckEtichetta))
                        End If
                    End If
                End If

            End If
        End If
        If listaErrori.Count > 0 Then
            Return listaErrori
        End If
#End Region

        'TODO_DT: da fare dopo MovimentiRiferimenti
        ''''Piva_Rif = hf_Piva_Rif.Value
        ''''Sa_Cod_Rif = hf_Sa_Cod_Rif.Value
        ''''Id_Agenda_Rif = hf_ID_Agenda_Rif.Value
        ''''Id_Mov_Rif = hf_ID_Mov_Rif.Value
        ''''Id_Mov_Det_Rif = hf_ID_Mov_Det_Rif.Value
        ''''Lav_Cod_Rif = hf_Lav_Cod_Rif.Value
        ''''Cau_Mov_Rif = hf_Cau_Mov_Rif.Value
        ''''Qta_Rif = hf_Qta_Rif.Value
        ''''Des_Rif = hf_Des_Rif.Value
        ''''If Piva_Rif <> "" AndAlso Id_Agenda_Rif <> "" AndAlso IsNumeric(Id_Agenda_Rif) AndAlso Id_Agenda_Rif <> 0 Then
        ''''    Dim objMov_Dettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        ''''    Dim dt_Det = objMov_Dettagli.Leggi(Piva_Rif, Sa_Cod_Rif, Id_Agenda_Rif, Id_Mov_Rif, Id_Mov_Det_Rif,
        ''''                                       0, 0, 0, "", 0, 0, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        ''''    If dt_Det.Rows.Count > 0 Then
        ''''        If CDec(CStr(Hidden_Dose_Tot_Reale.Value.Replace(".", AgronicaCoreUtility.Stringhe.SeparatoreDecimale))) > dt_Det.Rows(0)("Qta") Then
        ''''            Messaggi.AgroMsgBox("Non è possibile selezionare una Quantità superiore a " & CStr(dt_Det.Rows(0)("Qta")) & "", Page, , UpdatePanel_Prodotti)
        ''''            Exit Sub
        ''''        End If
        ''''    End If
        ''''End If

        ''''Qta_Rif = CStr(Hidden_Dose_Tot_Reale.Value.Replace(".", AgronicaCoreUtility.Stringhe.SeparatoreDecimale))

        Return listaErrori

    End Function

    Public Function getGridDosiManaged(input As Controllo_Inserimento_Dose_Prodotto) As List(Of RowGridDosi)

        'Un prodotto può esistere solo in una griglia di una operazione, impedito a monte da front end (poi ricontrollato in salvataggio)
        Dim gridDosiManaged As New List(Of RowGridDosi)
        For Each row_dose In input.row_grid_dosi
            If row_dose.DosiProdottiGridrowId <> input.DosiProdottiGridrowId Then 'escludo la riga in modifica/eliminazione
                Dim rowLotto = If(row_dose.Lotto IsNot Nothing, row_dose.Lotto, "")
                Dim rowPiva = If(row_dose.Piva IsNot Nothing, row_dose.Piva, "")

                Dim row_current = gridDosiManaged.Find(Function(d) d.Fr_Cod = row_dose.Fr_Cod AndAlso d.Lotto = rowLotto AndAlso d.Piva = rowPiva AndAlso d.Sa_Cod = row_dose.Sa_Cod AndAlso d.Fabbricato_Cod = row_dose.Fabbricato_Cod)
                If row_current Is Nothing Then
                    row_current = New RowGridDosi With {
                        .Piva = rowPiva,
                        .Sa_Cod = row_dose.Sa_Cod,
                        .Fabbricato_Cod = row_dose.Fabbricato_Cod,
                        .Lotto = rowLotto,
                        .Fr_Cod = row_dose.Fr_Cod,
                        .UdM = row_dose.UdM,
                        .Dosi_Etichetta = row_dose.Dosi_Etichetta,
                        .Avversita = row_dose.Avversita,
                        .Soglia_Avversita = row_dose.Soglia_Avversita,
                        .N = row_dose.N,
                        .N_Utile = row_dose.N_Utile,
                        .P = row_dose.P,
                        .K = row_dose.K,
                        .Cu = row_dose.Cu,
                        .Efficienza = row_dose.Efficienza,
                        .Polverulento = row_dose.Polverulento,
                        .Prodotto = row_dose.Prodotto,
                        .Dose_Ha = 0,
                        .Dose_Hl = 0,
                        .DoseTot_Ha = 0,
                        .Sup_Calcolata = 0,
                        .PrincipiAttivi = row_dose.PrincipiAttivi,
                        .Magazzino_Esterno = row_dose.Magazzino_Esterno
                    }
                    gridDosiManaged.Add(row_current)
                End If

                'posso sommare perchè mi sono assicurata a monte ci sia la stessa udm
                row_current.Dose_Ha += row_dose.Dose_Ha
                row_current.Dose_Hl += row_dose.Dose_Hl
                row_current.DoseTot_Ha += row_dose.DoseTot_Ha
                row_current.Sup_Calcolata += row_dose.Sup_Calcolata
            End If
        Next
        Return gridDosiManaged
    End Function

    Private Function BuildErrorMessage(errori As Dictionary(Of TipoErrore, List(Of String)), ByRef strErroreGlobale As String, titolo As String) As Boolean
        Dim isErroreBloccante = False

        Dim strErrore As String = ""

        If errori(TipoErrore.Bloccante).Count > 0 Then
            isErroreBloccante = True
            strErrore &= "<br>"
            For Each errMessage In errori(TipoErrore.Bloccante)
                strErrore &= errMessage & " (" & My.Resources.AgronicaCoreMapper.ControlloBloccante & ")" & "<br>"
            Next
        End If

        If errori(TipoErrore.BloccantePerImpostazioneUtente).Count > 0 Then
            isErroreBloccante = True
            strErrore &= "<br>"
            For Each errMessage In errori(TipoErrore.BloccantePerImpostazioneUtente)
                strErrore &= errMessage & " (" & My.Resources.AgronicaCoreMapper.ControlloBloccanteImpostazioneUtente & ")" & "<br>"
            Next
        End If

        If errori(TipoErrore.NonBloccante).Count > 0 Then
            strErrore &= "<br>"
            For Each errMessage In errori(TipoErrore.NonBloccante)
                strErrore &= errMessage & " (" & My.Resources.AgronicaCoreMapper.ControlloNonBloccante & ")" & "<br>"
            Next
        End If

        If errori(TipoErrore.NonBloccantePerImpostazioneUtente).Count > 0 Then
            strErrore &= "<br>"
            For Each errMessage In errori(TipoErrore.NonBloccantePerImpostazioneUtente)
                strErrore &= errMessage & " (" & My.Resources.AgronicaCoreMapper.ControlloNonBloccanteImpostazioneUtente & ")" & "<br>"
            Next
        End If

        If strErrore <> "" Then

            If titolo <> "" Then
                strErroreGlobale &= "<br><b>" & titolo & "</b>"
            End If
            strErroreGlobale &= strErrore

        End If

        Return isErroreBloccante

    End Function

    Private Sub FormatErroreGlobale(ByRef strErroreGlobale As String)
        If strErroreGlobale.StartsWith("<br>") Then
            strErroreGlobale = strErroreGlobale.Substring(4)
        End If

        If Not strErroreGlobale.EndsWith("<br>") Then
            strErroreGlobale &= "<br>"
        End If

    End Sub

    Public Sub ClearErrori(ByRef errori As Dictionary(Of TipoErrore, List(Of String)))
        errori(TipoErrore.Bloccante) = New List(Of String)
        errori(TipoErrore.BloccantePerImpostazioneUtente) = New List(Of String)
        errori(TipoErrore.NonBloccante) = New List(Of String)
        errori(TipoErrore.NonBloccantePerImpostazioneUtente) = New List(Of String)
    End Sub

    Private Sub CheckObbligatorieta(input As Controllo_Inserimento_Dose_Prodotto,
                                    infoOperazione As InfoOperazione,
                                    Parametri_Aggiuntivi_ControllaDosi As List(Of Parametri_Aggiuntivi_ControllaDosi),
                                    objParametri_Server As AgronicaCoreParametri,
                                    objParametri_Utenti As AgronicaCoreParametri,
                                    ByRef ListaErrori As List(Of ErroreGias))

        'IMPIANTI
        Select Case infoOperazione.TipoCentroDiCosto
            Case centri_di_costo.Tipo.ProdottoDaTrattare
                If input.row_grid_prodottiDaTrattare Is Nothing OrElse input.row_grid_prodottiDaTrattare.Count = 0 Then
                    ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", My.Resources.AgronicaCoreMapper.SelezionareProdottoDaTrattare, ""))
                End If
            Case Else
                If input.row_grid_impianti Is Nothing OrElse input.row_grid_impianti.Count = 0 Then
                    ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", My.Resources.AgronicaCoreMapper.SelezionareUnImpianto, ""))
                End If
        End Select

        'ACQUA 
        Utility.checkAcqua(input.risorsaAcqua, input.lavorazione, New List(Of Risorsa)({input.dettaglioTrattamento}), ListaErrori)

        'PRODOTTO IN INSERIMENTO
        Dim risorsaProdotto As New AgronicaCoreModelsSTD.attivita.risorse.RisorsaProdotto
        If infoOperazione.IsTrattamento Then
            risorsaProdotto = input.dettaglioTrattamento
        ElseIf infoOperazione.IsFertilizzazione Then
            risorsaProdotto = input.dettaglioFertilizzazione
        ElseIf infoOperazione.IsSemina Then
            risorsaProdotto = input.dettaglioSemina
        Else
            Exit Sub
        End If

        If risorsaProdotto.prodotto Is Nothing Then
            Dim messaggioErrore As String = ""
            If infoOperazione.IsTrattamento Then
                messaggioErrore = My.Resources.AgronicaCoreMapper.SelezionareUnFormulato
            End If
            If infoOperazione.IsFertilizzazione Then
                messaggioErrore = My.Resources.AgronicaCoreMapper.SelezionareUnFertilizzante
            End If
            If infoOperazione.IsSemina Then
                messaggioErrore = My.Resources.AgronicaCoreMapper.SelezionareUnaSementePiantina
            End If
            If Not String.IsNullOrEmpty(messaggioErrore) Then
                ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", messaggioErrore, ""))
            End If
        End If

        If infoOperazione.IsTrattamento Then
            Dim tipoFormulato_NoAvversita As Integer() = {enum_TipoFormulato.Coadiuvanti, enum_TipoFormulato.Corroboranti_Fisiofarmaci, enum_TipoFormulato.Fitoregolatori, enum_TipoFormulato.Disseccanti}
            'TODO Chiedere se va bene così il controllo
            If (Not tipoFormulato_NoAvversita.Contains(input.dettaglioTrattamento.tipoFormulato) AndAlso
                Not IsNothing(input.disciplinare) AndAlso input.disciplinare.codice <> enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta) OrElse
                (input.dettaglioTrattamento.tipoFormulato = enum_TipoFormulato.ConfusioneSessuale OrElse
                input.dettaglioTrattamento.tipoFormulato = enum_TipoFormulato.DisorientamentoSessuale) OrElse
                (infoOperazione.Elem_Cod = INSETTI AndAlso input.dettaglioTrattamento.isImpollinatore = False) Then

                If input.avversitaGruppo Is Nothing OrElse input.avversitaGruppo.codice = 0 Then
                    ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", My.Resources.AgronicaCoreMapper.ENecessarioSelezionareUnAvversità, ""))
                End If
            End If
        End If

        'DATI INSERITI IN PAGINA
        If input.doseHa < 0 Then
            ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", My.Resources.AgronicaCoreMapper.ImpossibileDoseHaNegativa, ""))
        End If

        If input.quantitaTotale < 0 Then
            ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", My.Resources.AgronicaCoreMapper.ImpossibileQuantitaProdottoNegativa, ""))
        End If

        If input.risorsaAcqua IsNot Nothing AndAlso input.risorsaAcqua.acqua < 0 Then
            ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", My.Resources.AgronicaCoreMapper.ImpossibileQuantitaAcquaNegativa, ""))
        End If

        If input.unitadiMisura Is Nothing OrElse input.unitadiMisura.codice = 0 Then
            ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", My.Resources.AgronicaCoreMapper.SelezionareUnUnitaDiMisura, ""))
        End If

        If infoOperazione.IsFertilizzazione = True Then

            If input.efficienza < 0 OrElse input.efficienza > 1 Then
                ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", My.Resources.AgronicaCoreMapper.InserireUnValoreDiEfficienzaIncluso01, ""))
            End If

            If input.N < 0 Then
                ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", My.Resources.AgronicaCoreMapper.NonEPossibileInserireUnaQuantitaDiNNegativ, ""))
            End If

            If input.N_Utile < 0 Then
                ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", My.Resources.AgronicaCoreMapper.NonEPossibileInserireUnaQuantitaDiNUtileNe, ""))
            End If

            If input.P < 0 Then
                ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", My.Resources.AgronicaCoreMapper.NonEPossibileInserireUnaQuantitaDiP2O5Nega, ""))
            End If

            If input.K < 0 Then
                ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", My.Resources.AgronicaCoreMapper.NonEPossibileInserireUnaQuantitaDiK2ONegat, ""))
            End If

            If input.Cu < 0 Then
                ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", My.Resources.AgronicaCoreMapper.NonEPossibileInserireUnaQuantitaDiCuNegat, ""))
            End If

        End If

        If infoOperazione.IsSemina = True Then
            'SEMINA / TRAPIANTO CON AGGIORNAMENTO ANAGRAFICA / FRAZIONAMENTO:
            'Controllo che il prodotto selezionato abbia una specie indicata
            For Each parametroAggiuntivo In Parametri_Aggiuntivi_ControllaDosi
                If parametroAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.Opzione_Semina Then
                    If parametroAggiuntivo.value = enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default OrElse
                        parametroAggiuntivo.value = enum_SEMINA_TIPO.Semina_e_Modifica_Appezzamenti_Default Then
                        Dim DettaglioSemina = CType(risorsaProdotto, dettagli.DettaglioSemina)
                        If DettaglioSemina.varieta.specie.codice = 0 Then
                            ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(Gias.ImpossibileUtilizzareProdottoXSpecieNonImpostataAnagrafica, risorsaProdotto.prodotto.descrizione), ""))
                        End If
                    End If
                End If
            Next
        End If

        Utility.Controllo_Compatibilita_Impostazioni_Tra_Azienda_QdC_Esterna(input.Visualizza_Magazzini_Esterni, input.impresa.ragioneSociale, input.Categoria_Magazzino,
                                                                             input.Magazzino, input.Magazzino_Esterno, input.Lotto, objParametri_Server, objParametri_Utenti, ListaErrori)
    End Sub

    Private Sub CheckCompatibilita(input As Controllo_Inserimento_Dose_Prodotto,
                                   gridDosiManaged As List(Of RowGridDosi),
                                   infoOperazione As InfoOperazione,
                                   ByRef ListaErrori As List(Of ErroreGias))

        Dim erroreGias As ErroreGias = Nothing

        Dim risorsaProdotto As New AgronicaCoreModelsSTD.attivita.risorse.RisorsaProdotto
        If infoOperazione.IsTrattamento Then
            risorsaProdotto = input.dettaglioTrattamento
        ElseIf infoOperazione.IsFertilizzazione Then
            risorsaProdotto = input.dettaglioFertilizzazione
        ElseIf infoOperazione.IsSemina Then
            risorsaProdotto = input.dettaglioSemina
        Else
            Exit Sub
        End If

        For Each prodottoGriglia In gridDosiManaged.FindAll(Function(p) p.Fr_Cod = risorsaProdotto.prodotto.codice)

            'UDM
            If input.unitadiMisura.codice <> prodottoGriglia.UdM.codice Then
                ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.SelezionareUnitaDiMisuraUniche, risorsaProdotto.prodotto.descrizione), ""))
                Exit For
            End If

            If infoOperazione.IsTrattamento Then

                'CLASSIFICAZIONE
                If input.dettaglioTrattamento.classificazioni <> CType(prodottoGriglia.Prodotto, dettagli.DettaglioTrattamento).classificazioni Then
                    ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.ClassificazioniNonCompatibili, risorsaProdotto.prodotto.descrizione), ""))
                    Exit For
                End If

                'AVVERSITA e SOGLIA
                Dim av_cod_input As Integer = 0
                Dim av_gru_input As Integer = 0
                STD_Utility.GetCodiciAvversita(input.avversitaGruppo, input.dettaglioTrattamento.tipoFormulato, av_cod_input, av_gru_input)

                Dim av_cod_griglia As Integer = 0
                Dim av_gru_griglia As Integer = 0
                STD_Utility.GetCodiciAvversita(prodottoGriglia.Avversita, input.dettaglioTrattamento.tipoFormulato, av_cod_griglia, av_gru_griglia)

                If av_cod_input <> av_cod_griglia OrElse av_gru_input <> av_gru_griglia Then
                    ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.SelezionareAvversitaUniche, risorsaProdotto.prodotto.descrizione), ""))
                    Exit For
                End If

                If input.Soglia_Avversita?.codice <> prodottoGriglia.Soglia_Avversita?.codice OrElse input.Soglia_Avversita?.quantita <> prodottoGriglia.Soglia_Avversita?.quantita Then
                    ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.SelezionareSoglieUniche, risorsaProdotto.prodotto.descrizione), ""))
                    Exit For
                End If

                'DOSI ETICHETTA
                If Not Utility.checkDosiEtichettaUguali(input.dosi_Etichetta, prodottoGriglia.Dosi_Etichetta) Then
                    ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.SelezionareDosiEtichettaUniche, risorsaProdotto.prodotto.descrizione), ""))
                    Exit For
                End If

            End If

            If infoOperazione.IsFertilizzazione Then

                'N P K  CU + Efficienza
                If input.N <> prodottoGriglia.N OrElse input.P <> prodottoGriglia.P OrElse input.K <> prodottoGriglia.K OrElse input.Cu <> prodottoGriglia.Cu OrElse input.efficienza <> prodottoGriglia.Efficienza Then
                    ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.SelezionareTitoliUnici, risorsaProdotto.prodotto.descrizione), ""))
                    Exit For
                End If

            End If

            If infoOperazione.IsSemina Then
                'non possono esistere movimenti con stesso prodotto/lotto su magazzini diversi
                If input.Lotto = prodottoGriglia.Lotto Then
                    If input.Magazzino IsNot Nothing AndAlso input.Magazzino.primaryKey IsNot Nothing AndAlso input.Magazzino.primaryKey.centroAziendalePK IsNot Nothing Then
                        If input.Magazzino.primaryKey.codice <> prodottoGriglia.Fabbricato_Cod OrElse input.Magazzino.primaryKey.centroAziendalePK.codice <> prodottoGriglia.Sa_Cod OrElse input.Magazzino.primaryKey.centroAziendalePK.partitaIva <> prodottoGriglia.Piva Then
                            ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.SeminaLottoEsistente, input.Lotto, risorsaProdotto.prodotto.descrizione), ""))
                            Exit For
                        End If
                    End If
                End If
            End If


            'Casi in qui viene bloccato l'inserimento con magazzino esterno:
            '-	Scarico dello stesso prodotto contemporaneamente dal magazzino interno e da uno esterno
            '-	Scarico dello stesso prodotto contemporaneamente da magazzini di diverse aziende esterne 
            '-	Scarico dello stesso prodotto contemporaneamente da magazzini diversi della stessa azienda esterna
            If input.Lotto.ToUpper() = prodottoGriglia.Lotto.ToUpper() Then
                If input.Magazzino IsNot Nothing AndAlso input.Magazzino.primaryKey IsNot Nothing AndAlso input.Magazzino.primaryKey.centroAziendalePK IsNot Nothing Then
                    If input.Magazzino.primaryKey.codice = prodottoGriglia.Fabbricato_Cod AndAlso input.Magazzino.primaryKey.centroAziendalePK.codice = prodottoGriglia.Sa_Cod AndAlso input.Magazzino.primaryKey.centroAziendalePK.partitaIva = prodottoGriglia.Piva Then

                        If Not IsNothing(prodottoGriglia.Magazzino_Esterno) AndAlso Not IsNothing(input.Magazzino_Esterno) AndAlso
                                    Not (input.Magazzino_Esterno.primaryKey.codice = prodottoGriglia.Magazzino_Esterno.primaryKey.codice AndAlso
                                    input.Magazzino_Esterno.primaryKey.centroAziendalePK.codice = prodottoGriglia.Magazzino_Esterno.primaryKey.centroAziendalePK.codice AndAlso
                                    input.Magazzino_Esterno.primaryKey.centroAziendalePK.partitaIva = prodottoGriglia.Magazzino_Esterno.primaryKey.centroAziendalePK.partitaIva) Then


                            ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.NonePossibileInserireloStessoProdottopresoDaMagazEsterniDiversi, input.Lotto, input.Magazzino.descrizione, risorsaProdotto.prodotto.descrizione), ""))
                            Exit For

                        ElseIf (Not IsNothing(input.Magazzino_Esterno) AndAlso IsNothing(prodottoGriglia.Magazzino_Esterno)) OrElse
                                (IsNothing(input.Magazzino_Esterno) AndAlso Not IsNothing(prodottoGriglia.Magazzino_Esterno)) Then

                            ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.NonePossibileInserireloStessoProdottopresoDaMagazEsterniInterni, input.Lotto, input.Magazzino.descrizione, risorsaProdotto.prodotto.descrizione), ""))
                            Exit For

                        End If
                    End If
                End If
            End If

        Next

        If infoOperazione.IsSemina Then

            'verifico che la superficie dei nuovi appezzamenti non superi la superficie trattata totale
            Dim tipoSemina = 0
            If input.tipo_Semina IsNot Nothing Then
                tipoSemina = input.tipo_Semina.codice
            End If
            If tipoSemina = enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default Then

                Dim sup_calcolata_totale As Decimal = 0
                For Each prodottoGriglia In gridDosiManaged
                    sup_calcolata_totale += prodottoGriglia.Sup_Calcolata 'sup dei prodotti già in griglia (esclusi quelli in modifica/eliminazione)
                Next
                sup_calcolata_totale += input.sup_Calcolata 'sup del prodotto che si sta inserendo

                If sup_calcolata_totale > input.sup_Trattata Then
                    ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", My.Resources.AgronicaCoreMapper.SuperficieCalcolataSuperaIndicata, ""))
                End If
            End If
        End If

    End Sub

    Private Sub CheckGiacenzaMagazzino(input As Controllo_Inserimento_Dose_Prodotto,
                                       gridDosiManaged As List(Of RowGridDosi),
                                       infoOperazione As InfoOperazione,
                                       ByRef errori As Dictionary(Of TipoErrore, List(Of String)),
                                       objParametri_Utenti As AgronicaCoreParametri,
                                       objParametri_Server As AgronicaCoreParametri)


        If input.Magazzino IsNot Nothing OrElse input.Magazzino_Innesco IsNot Nothing Then

            Dim DictErroriTempXElemCod As New Dictionary(Of Integer, List(Of ErroreGias))

            'DT: in caso di multicentro/multioperazione, si prende il pirmo id_agenda dai parametri aggiuntivi
            'sarà poi il verifica giacenze che esclude il relativo raccoglitore, che è uguale per tutte le agende del multicentro
            Dim id_agenda_da_non_considerare As Integer = 0
            Dim listaAgendeMulticentro = Utility.GetAgendeMulticentroFromParametri(input.parametri_aggiuntivi_list)
            If listaAgendeMulticentro IsNot Nothing AndAlso listaAgendeMulticentro.Count > 0 Then
                id_agenda_da_non_considerare = listaAgendeMulticentro(0)
            Else
                id_agenda_da_non_considerare = input.id_agenda
            End If

            Dim Piva_agenda = ""

            Select Case infoOperazione.TipoCentroDiCosto
                Case centri_di_costo.Tipo.ProdottoDaTrattare
                    Piva_agenda = input.row_grid_prodottiDaTrattare(0).PIVA
                Case Else
                    Piva_agenda = input.row_grid_impianti(0).PIVA
            End Select

            Dim Piva_fabbricato As String = ""
            Dim Sa_Cod_fabbricato As Integer = 0
            Dim Fabbricato_Cod As Integer = 0
            Dim descrizioneMagazzino As String = ""

            Dim codiceProdotto As Integer = 0
            Dim descrizioneProdotto As String = ""
            Dim elemCod As Integer = 0

            Dim QtaTot As Decimal = 0

            Dim udmBase As UnitaDiMisura = Nothing

            If input.Magazzino IsNot Nothing Then

                Piva_fabbricato = input.Magazzino.primaryKey.centroAziendalePK.partitaIva
                Sa_Cod_fabbricato = input.Magazzino.primaryKey.centroAziendalePK.codice
                Fabbricato_Cod = input.Magazzino.primaryKey.codice
                descrizioneMagazzino = input.Magazzino.descrizione

                If input.Magazzino_Esterno IsNot Nothing Then
                    Piva_fabbricato = input.Magazzino_Esterno.primaryKey.centroAziendalePK.partitaIva
                    Sa_Cod_fabbricato = input.Magazzino_Esterno.primaryKey.centroAziendalePK.codice
                    Fabbricato_Cod = input.Magazzino_Esterno.primaryKey.codice
                    descrizioneMagazzino = input.Magazzino_Esterno.descrizione
                End If

                Dim effCod As Integer = 0

                Dim risorsaProdotto As New RisorsaProdotto

                If infoOperazione.IsTrattamento Then
                    risorsaProdotto = CType(input.dettaglioTrattamento, RisorsaProdotto)
                End If
                If infoOperazione.IsFertilizzazione Then
                    risorsaProdotto = CType(input.dettaglioFertilizzazione, RisorsaProdotto)
                    effCod = If(input.dettaglioFertilizzazione.effluente IsNot Nothing, input.dettaglioFertilizzazione.effluente.codice, 0)
                End If
                If infoOperazione.IsSemina Then
                    risorsaProdotto = CType(input.dettaglioSemina, RisorsaProdotto)
                End If

                codiceProdotto = risorsaProdotto.prodotto.codice
                descrizioneProdotto = risorsaProdotto.prodotto.descrizione
                elemCod = risorsaProdotto.prodotto.elemCod

                QtaTot = input.quantitaTotale

                Dim row_stesso_prodotto = gridDosiManaged.Find(Function(d) d.Fr_Cod = codiceProdotto AndAlso d.Lotto = input.Lotto AndAlso d.Piva = Piva_fabbricato AndAlso d.Sa_Cod = Sa_Cod_fabbricato AndAlso d.Fabbricato_Cod = Fabbricato_Cod)
                If row_stesso_prodotto IsNot Nothing Then
                    QtaTot += row_stesso_prodotto.DoseTot_Ha 'le udm sono equivalenti perchè controllate a monte, posso sommare
                End If
                udmBase = STD_Utility.getUdmBasefromUdmIndicata(input.unitadiMisura, QtaTot, objParametri_Server)

                Dim pua As Pua = input.pua

                'TODO Da Verificare con Monti e correggere:
                'Ora nel Verifica_Giacenze_Con_Pua per ottenere le qta alla data e totale fa una sottrazione tra la 
                'qta indicata nel PUA (con unita di misura che sarà quintali o m3) e la qta delle ricette (con unita di misura che kg o l)
                'senza fare prima un'adeguata conversione
                If Not IsNothing(input.pua) AndAlso input.pua.codice > 0 AndAlso
                    input.lavorazione.primaryKey.codice = LAVCOD_DISTRIBUZIONE_AMMENDANTI AndAlso
                    input.tipoAttivita = Attivita.Tipo_Attivita.Ricetta AndAlso
                    input.tipoRicetta > 0 AndAlso
                    input.statoAttivita = Attivita.Stati.Da_Eseguire AndAlso
                    Not IsNothing(input.disciplinare) AndAlso
                    Not IsNothing(input.disciplinare.regolamentoConcimazione) AndAlso
                    input.disciplinare.regolamentoConcimazione.tipo = enum_PUARegolamenti_Tipo.PUA Then

                    Dim Qta_KG_L_Effluente_PUA As Decimal = input.dettaglioFertilizzazione.effluente.carico
                    STD_Utility.getUdmBasefromUdmIndicata(input.dettaglioFertilizzazione.effluente.udm, Qta_KG_L_Effluente_PUA, objParametri_Server)
                    'guardo se la quantità è conforme per la data di intervento

                    Dim qta_in_data As Decimal = 0

                    Dim objRicDet As New AgronicaCoreContabDAL.Ricette_Dettagli_R
                    qta_in_data = objRicDet.Verifica_Giacenze_KG_L_Con_Pua_NEW(
                                                Qta_KG_L_Effluente_PUA,
                                                Piva_agenda,
                                                input.ricetta_cod,
                                                elemCod,
                                                If(infoOperazione.IsSemina, 0, codiceProdotto),
                                                If(infoOperazione.IsSemina, codiceProdotto, 0),
                                                AGRODATAINIZIO,
                                                input.Data,
                                                input.ricetta_operazione_cod,
                                                objParametri_Server)

                    If QtaTot > Math.Round(qta_in_data, 4) Then

                        GestisciDictErroriXElemCod(elemCod,
                                                    generaErroreGias(ErroreGias_Severity.Warning, "", String.Format(My.Resources.AgronicaCoreMapper.QuantitaDichiaratoPUANonSuffScarico, descrizioneProdotto, CDate(input.Data).ToShortDateString, Math.Round(qta_in_data, 4), udmBase.descrizione), ""),
                                                    DictErroriTempXElemCod)
                    End If

                    'guardo se la quantità di giacenza è conforme a prescindere dalla data
                    qta_in_data = objRicDet.Verifica_Giacenze_KG_L_Con_Pua_NEW(
                                                Qta_KG_L_Effluente_PUA,
                                                Piva_agenda,
                                                input.ricetta_cod,
                                                elemCod,
                                                If(infoOperazione.IsSemina, 0, codiceProdotto),
                                                If(infoOperazione.IsSemina, codiceProdotto, 0),
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                input.ricetta_operazione_cod,
                                                objParametri_Server)

                    If QtaTot > Math.Round(qta_in_data, 4) Then

                        GestisciDictErroriXElemCod(elemCod,
                                                  generaErroreGias(ErroreGias_Severity.Warning, "", String.Format(My.Resources.AgronicaCoreMapper.GiacenzaDichiaratoPUANonSuffScarico, descrizioneProdotto, Math.Round(qta_in_data, 4), udmBase.descrizione), ""),
                                                    DictErroriTempXElemCod)
                    End If

                End If

                CheckGiacenzaMagazzinoInternal(Piva_agenda, id_agenda_da_non_considerare, Piva_fabbricato, Sa_Cod_fabbricato, Fabbricato_Cod,
                                descrizioneMagazzino, udmBase, elemCod, codiceProdotto, descrizioneProdotto,
                                QtaTot, input.Lotto, input.Data, infoOperazione, objParametri_Server, DictErroriTempXElemCod)

            End If

            If input.Magazzino_Innesco IsNot Nothing Then

                Piva_fabbricato = input.Magazzino_Innesco.primaryKey.centroAziendalePK.partitaIva
                Sa_Cod_fabbricato = input.Magazzino_Innesco.primaryKey.centroAziendalePK.codice
                Fabbricato_Cod = input.Magazzino_Innesco.primaryKey.codice
                descrizioneMagazzino = input.Magazzino_Innesco.descrizione

                If infoOperazione.IsTrattamento Then

                    If input.avversitaGruppo IsNot Nothing Then
                        codiceProdotto = input.avversitaGruppo.codice
                        descrizioneProdotto = input.avversitaGruppo.descrizione
                        elemCod = INNESCHI
                    End If

                    QtaTot = input.DoseTot_Ha_Innesco

                    Dim row_stesso_prodotto = gridDosiManaged.Find(Function(d) d.Avversita.codice = codiceProdotto AndAlso d.Lotto_Innesco = input.Lotto_Innesco AndAlso
                                                                       d.Piva_Innesco = Piva_fabbricato AndAlso d.Sa_Cod_Innesco = Sa_Cod_fabbricato AndAlso d.Fabbricato_Cod_Innesco = Fabbricato_Cod)
                    If row_stesso_prodotto IsNot Nothing Then
                        QtaTot += row_stesso_prodotto.DoseTot_Ha_Innesco 'le udm sono equivalenti perchè controllate a monte, posso sommare
                    End If
                    udmBase = STD_Utility.getUdmBasefromUdmIndicata(input.UdM_Innesco, QtaTot, objParametri_Server)

                    CheckGiacenzaMagazzinoInternal(Piva_agenda, id_agenda_da_non_considerare, Piva_fabbricato, Sa_Cod_fabbricato, Fabbricato_Cod,
                                                        descrizioneMagazzino, udmBase, elemCod, codiceProdotto, descrizioneProdotto,
                                                        QtaTot, input.Lotto_Innesco, input.Data, infoOperazione, objParametri_Server, DictErroriTempXElemCod)

                End If

            End If


            If DictErroriTempXElemCod.Count > 0 Then

                Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim bloccaGiacenze_Utente = False
                If (objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE, objParametri_Utenti.UtenteUsername, objParametri_Utenti)) <> "0" Then
                    bloccaGiacenze_Utente = True
                End If

                Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

                Dim tipoOperazione As enum_Tipo_Operazione_Agenda = STD_Utility.getTipoOperazione(input.tipoAttivita, input.statoAttivita)

                For Each DictElemCod In DictErroriTempXElemCod.Keys

                    For Each errore In DictErroriTempXElemCod.Item(DictElemCod)

                        If bloccaGiacenze_Utente Then

                            errore.severity = ErroreGias_Severity.WarningBloccante

                        ElseIf tipoOperazione = enum_Tipo_Operazione_Agenda.Ricetta Then

                            'Il controllo di giacenza per le ricette non può essere bloccante
                            errore.severity = ErroreGias_Severity.Warning

                        Else
                            'DT: bisognerebbe prendere i centri dei fabbricati, non quelli degli impianti.
                            'ma si è valutato di fermarsi a livello di super user o azienda, quindi per il momento è sufficiente passare la PIVA (del fabbricato)
                            Dim GestioneGiacenze = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(Piva_fabbricato, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, DictElemCod, enum_Gestione_Giacenze.TuttiProdotti, objParametri_Utenti, objParametri_Server))
                            If GestioneGiacenze = enum_Gestione_Giacenze.SoloPresenti Then
                                errore.severity = ErroreGias_Severity.WarningBloccante
                            End If
                        End If

                    Next

                Next

                For Each erroriList In DictErroriTempXElemCod.Values

                    For Each errore In erroriList
                        If errore.severity = ErroreGias_Severity.WarningBloccante Then
                            errori(TipoErrore.BloccantePerImpostazioneUtente).Add(errore.messaggio)
                        Else
                            errori(TipoErrore.NonBloccantePerImpostazioneUtente).Add(errore.messaggio)
                        End If
                    Next

                Next

            End If

        End If

    End Sub

    Private Sub CheckCarenza(input As Controllo_Inserimento_Dose_Prodotto, ByRef ListaErroriTemp As List(Of ErroreGias), objParametri_Utenti As AgronicaCoreParametri)

        If input.dettaglioTrattamento.tempoCarenza > 0 AndAlso input.disciplinare IsNot Nothing AndAlso input.disciplinare.codice <> enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta Then

            Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

            Dim Blocca_Carenza As Boolean = False
            If (objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA, objParametri_Utenti.UtenteUsername, objParametri_Utenti)) = "1" Then
                Blocca_Carenza = True
            End If

            If Blocca_Carenza Then

                Dim DataMinimaRaccolta = AGRODATAFINE

                For i = 0 To input.row_grid_impianti.Count - 1

                    Dim temp As Date
                    Dim Data_Raccolta = If(Date.TryParse(input.row_grid_impianti(i).Data_Raccolta, temp), temp, New Date)
                    Dim Data_Raccolta_Prevista = If(Date.TryParse(input.row_grid_impianti(i).Data_Raccolta_Prevista, temp), temp, New Date)

                    If Data_Raccolta <> New Date Then
                        If DataMinimaRaccolta > Data_Raccolta AndAlso Data_Raccolta >= input.Data Then
                            DataMinimaRaccolta = Data_Raccolta
                        End If
                    Else
                        If Data_Raccolta_Prevista <> New Date Then
                            If DataMinimaRaccolta > Data_Raccolta_Prevista AndAlso Data_Raccolta_Prevista >= input.Data Then
                                DataMinimaRaccolta = Data_Raccolta_Prevista
                            End If
                        End If
                    End If

                Next

                Dim DataMinimaRaccoltaPerCarenza = CDate(input.Data).AddDays(input.dettaglioTrattamento.tempoCarenza + 1)

                'se devo ancora raccogliere e la carenza dovuta al trattamento è successiva alla data minima di raccolta, errore
                If input.Data <= DataMinimaRaccolta AndAlso DataMinimaRaccolta < DataMinimaRaccoltaPerCarenza Then
                    ListaErroriTemp.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.LaDataDiRaccoltaDiUnoDegliAppezzamentiNonÈ, DataMinimaRaccolta.ToShortDateString, input.dettaglioTrattamento.tempoCarenza), ""))
                End If

            End If
        End If
    End Sub

    Private Sub CheckBloccoFioritura(input As Controllo_Inserimento_Dose_Prodotto, ByRef ListaErroriTemp As List(Of ErroreGias), objParametri_Utenti As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri, objParametri_Super_Server As AgronicaCoreParametri)

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim Blocca_Fioritura = False
        If (objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA, objParametri_Utenti.UtenteUsername, objParametri_Utenti)) = "1" Then
            Blocca_Fioritura = True
        End If

        If Not String.IsNullOrEmpty(input.dettaglioTrattamento.epocheBlocchi) AndAlso Blocca_Fioritura Then

            Dim Intervalli() As String = input.dettaglioTrattamento.epocheBlocchi.Split("|")

            If Intervalli IsNot Nothing Then

                Dim Veg_Cod = Utility.GetVegCodFromUtilizzoTerreno(input.utilizzoTerreno)

                '(12/11/2018 fede) aggiunta indicazione fase fenologica corrente
                Dim objParametriUscitaFasiNew As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output

                If Veg_Cod > 0 Then
                    Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input With {
                        .Veg_Cod = Veg_Cod
                    }
                    Dim Leggi_impostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                    Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE, objParametri_Utenti, 2)
                    If imp = "1" Then
                        objParametriIngresso.Personalizzate = True
                    End If

                    Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

                    Dim DTConfigurazioneSiti As DataTable = objConfigurazione_Siti.Leggi(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)

                    If Not IsNothing(DTConfigurazioneSiti) AndAlso DTConfigurazioneSiti.Rows.Count = 1 AndAlso DTConfigurazioneSiti.Rows(0)("Valore") <> "" Then
                        objParametriIngresso.Url = DTConfigurazioneSiti.Rows(0)("Valore") & "/FasiFenologiche"
                    End If

                    Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS
                    objParametriUscitaFasiNew = objFasi_WS.FasiFenologiche(objParametriIngresso)
                End If

                For Inter = 0 To Intervalli.Length - 1

                    Dim Epoca_DA As Integer = 0
                    Dim Stadio_Da As String = ""
                    Dim Progressivo_Da As Integer = 0
                    Dim Epoca_A As Integer = 0
                    Dim Stadio_A As String = ""
                    Dim Progressivo_A As Integer = 0

                    If IsNumeric(Split(Intervalli(Inter), "_")(0)) Then
                        Epoca_DA = CInt(Split(Intervalli(Inter), "_")(0))
                    End If
                    If Split(Intervalli(Inter), "_")(1) Then
                        Stadio_Da = CInt(Split(Intervalli(Inter), "_")(1))
                    End If
                    If IsNumeric(Split(Intervalli(Inter), "_")(2)) Then
                        Epoca_A = CInt(Split(Intervalli(Inter), "_")(2))
                    End If
                    If Split(Intervalli(Inter), "_")(3) Then
                        Stadio_A = CInt(Split(Intervalli(Inter), "_")(3))
                    End If

                    Dim Fase_Cod_Corrente As Integer = 0
                    Dim StadioBbch_Corrente As String
                    For i = 0 To input.row_grid_impianti.Count - 1
                        Dim objDest As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                        Dim Dt As New DataTable
                        Dim Piva = input.row_grid_impianti(i).PIVA
                        Dim Sa_Cod = input.row_grid_impianti(i).SA_COD
                        Dim Appezza = input.row_grid_impianti(i).APPEZZA
                        Dim Id_Reg = input.row_grid_impianti(i).ID_REG
                        Dt = objDest.Leggi_UltimaFaseFenologicaImpianti_InData(Piva, Sa_Cod, Appezza, Id_Reg, input.Data, "", "", objParametri_Server)
                        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then
                            Fase_Cod_Corrente = Dt.Rows(0).Item("FF_Classe")
                            If Fase_Cod_Corrente >= 1000 Then 'caso nuovo
                                StadioBbch_Corrente = (From aa In objParametriUscitaFasiNew.ListaFasiFenologiche
                                                       Where aa.Cod_SS = Fase_Cod_Corrente
                                                       Select aa.Stadio
                                                        ).FirstOrDefault
                            End If
                        End If

                        Dim foundErrore = False
                        'blocco da .....
                        If Epoca_DA <> 0 And Epoca_A = 0 Then
                            If Stadio_Da < StadioBbch_Corrente Then
                                foundErrore = True
                            End If

                            'blocco fino a .................
                        ElseIf Epoca_DA = 0 And Epoca_A <> 0 Then
                            If Stadio_A > StadioBbch_Corrente Then
                                foundErrore = True
                            End If

                            'blocco all'interno
                        ElseIf Epoca_DA <> 0 And Epoca_A <> 0 Then
                            If Stadio_A > StadioBbch_Corrente Or Stadio_Da < StadioBbch_Corrente Then
                                foundErrore = True
                            End If
                        End If

                        If foundErrore Then
                            ListaErroriTemp.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", My.Resources.AgronicaCoreMapper.BloccoFiorituraNonRispettato, ""))
                        End If
                    Next
                Next
            End If
        End If
    End Sub

    Private Sub CheckBuffer(
        input As Controllo_Inserimento_Dose_Prodotto,
        ByRef ListaErroriTemp As List(Of ErroreGias),
        objParametri_Utenti As AgronicaCoreParametri
    )
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim Blocca_BufferZone = False
        If (objUtentiImpostazioni.LeggiValoreImpostazioneScalare(
            enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA,
            objParametri_Utenti.UtenteUsername, objParametri_Utenti)) = "1" Then

            Blocca_BufferZone = True
        End If

        Dim BufferMin As Decimal = 0
        Dim BufferMax As Decimal = 0
        If input.dettaglioTrattamento.bufferzone IsNot Nothing Then
            BufferMin = input.dettaglioTrattamento.bufferzone.minimo
            BufferMax = input.dettaglioTrattamento.bufferzone.massimo
        End If

        If Blocca_BufferZone AndAlso (BufferMin <> 0 OrElse BufferMax <> 0) Then

            For i = 0 To input.row_grid_impianti.Count - 1

                Dim lunghezza As Decimal = input.row_grid_impianti(i).DistBZ_CorpiIdrici + input.row_grid_impianti(i).DistBZ_AreeResPub + input.row_grid_impianti(i).DistBZ_Allevamenti + input.row_grid_impianti(i).DistBZ_VegNatNonColt

                If lunghezza > 0 Then

                    Dim distanzaprimapianta As Decimal = input.row_grid_impianti(i).SupBZ_Riduzione 'capezzagna
                    Dim perc_mitig As Decimal = input.row_grid_impianti(i).Perc_Riduzione_Deriva

                    Dim buffer_da_controllare As Decimal = (BufferMin - distanzaprimapianta) - ((BufferMin - distanzaprimapianta) * perc_mitig / 100)

                    Dim sup_rid As Decimal = 0
                    If (lunghezza * buffer_da_controllare / 10000) > 0 Then
                        sup_rid = lunghezza * buffer_da_controllare / 10000
                    End If

                    Dim sup_massima_trattabile As Decimal = Agro_Math.ArrotondaVal_4(input.row_grid_impianti(i).Sup_Imp - sup_rid)

                    If sup_massima_trattabile > 0 AndAlso input.row_grid_impianti(i).Sup_Imp_help > sup_massima_trattabile Then
                        If BufferMin <> 0 And BufferMax = 0 Then
                            ListaErroriTemp.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.BufferMinNonRispettata, BufferMin), ""))
                        Else
                            ListaErroriTemp.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.BufferMinMaxNonRispettata, BufferMin, BufferMax), ""))
                        End If
                    End If
                End If
            Next
        End If

    End Sub

    Private Sub CheckPeriodoDivieto(input As Controllo_Inserimento_Dose_Prodotto, ByRef errori As Dictionary(Of TipoErrore, List(Of String)), objParametri_Server As AgronicaCoreParametri)

        Dim effCod As Integer = 0
        If input.dettaglioFertilizzazione.effluente IsNot Nothing AndAlso input.dettaglioFertilizzazione.effluente.codice > 0 Then
            effCod = input.dettaglioFertilizzazione.effluente.codice
        End If

        Dim Pua_Cod As Integer = 0
        Dim Regolamento_Cod_PUA As Integer = 0

        If Not IsNothing(input.pua) AndAlso input.pua.codice > 0 Then
            Pua_Cod = input.pua.codice

            If Not IsNothing(input.pua.disciplinare) Then
                If Not IsNothing(input.pua.disciplinare.regolamentoConcimazione) Then
                    Regolamento_Cod_PUA = input.pua.disciplinare.regolamentoConcimazione.codice
                Else
                    Regolamento_Cod_PUA = input.pua.disciplinare.codice
                End If
            End If
        End If

        Dim objEffDiv As New AgronicaCorePUA_DAL.Pua_Effluente_PeriodoDivieto_R
        Dim DtEffluentiDiv As DataTable = objEffDiv.Leggi_DaEffluente(Regolamento_Cod_PUA, Pua_Cod, 0, 0, effCod, "", "", objParametri_Server)

        If DtEffluentiDiv IsNot Nothing AndAlso DtEffluentiDiv.Rows.Count > 0 Then
            Dim Data_Inizio_Divieto As Date = AGRODATAINIZIO
            Dim Data_Fine_Divieto As Date = AGRODATAFINE

            For i = 0 To DtEffluentiDiv.Rows.Count - 1

                If Not IsDBNull(DtEffluentiDiv.Rows(i).Item("Data_Divieto_DA")) Then
                    Data_Inizio_Divieto = DtEffluentiDiv.Rows(i).Item("Data_Divieto_DA")
                End If
                If Not IsDBNull(DtEffluentiDiv.Rows(i).Item("Data_Divieto_A")) Then
                    Data_Fine_Divieto = DtEffluentiDiv.Rows(i).Item("Data_Divieto_A")
                End If

                If Data_Inizio_Divieto <> AGRODATAINIZIO AndAlso input.Data >= Data_Inizio_Divieto AndAlso
                    Data_Fine_Divieto <> AGRODATAFINE AndAlso input.Data <= Data_Fine_Divieto Then
                    Dim messaggio = String.Format(My.Resources.AgronicaCoreMapper.PeriodoDivietoNonRispettato, Data_Inizio_Divieto.ToShortDateString, Data_Fine_Divieto.ToShortDateString)
                    errori(TipoErrore.NonBloccante).Add(messaggio)
                End If
            Next
        End If

    End Sub

    Private Sub CheckDosaggio(doseEtichetta As DoseEtichetta,
                              input As Controllo_Inserimento_Dose_Prodotto,
                              gridDosiManaged As List(Of RowGridDosi),
                              Blocca_Inserimento As String,
                              infoOperazione As InfoOperazione,
                              ByRef errori As Dictionary(Of TipoErrore, List(Of String)),
                              objParametri_Utenti As AgronicaCoreParametri,
                              objParametri_Server As AgronicaCoreParametri)

        Dim doseHaTotale = input.doseHa
        Dim doseHlTotale = input.doseHl
        Dim doseQTotale = input.doseQ

        For Each prodottoGriglia In gridDosiManaged.FindAll(Function(p) p.Fr_Cod = input.dettaglioTrattamento.prodotto.codice)
            doseHaTotale += prodottoGriglia.Dose_Ha
            doseHlTotale += prodottoGriglia.Dose_Hl
            doseQTotale += prodottoGriglia.Dose_Q
        Next

        CheckDoseMassima(doseEtichetta, input.unitadiMisura, input.DoseConsentitaDiserbo, doseHaTotale, doseHlTotale, doseQTotale, Blocca_Inserimento, errori, objParametri_Utenti)
        CheckDoseMinima(doseEtichetta, input.unitadiMisura, doseHaTotale, doseHlTotale, doseQTotale, Blocca_Inserimento, errori, objParametri_Utenti)

        'TODO ANNA 5/11
        'Per il momento si fa il controllo sul numero di trattamenti solo per le operazioni che hanno come centro di costo l'esercizio
        'Da studiare per i prodotti da trattare ( TRATTAMENTI POST RACCOLTA e CONCIA DEL SEME)
        'Sarà da adeguare anche il WebService
        If infoOperazione.TipoCentroDiCosto = centri_di_costo.Tipo.Esercizio Then
            CheckNumeroMaxTrattamenti(doseEtichetta, input, Blocca_Inserimento, errori, objParametri_Utenti, objParametri_Server)
            CheckIntervalloTrattamenti(doseEtichetta, input, Blocca_Inserimento, errori, objParametri_Utenti, objParametri_Server)
        End If

        CheckAcqua(doseEtichetta, input, Blocca_Inserimento, infoOperazione, errori, objParametri_Utenti, objParametri_Server)

    End Sub

    Private Sub CheckMixPolverulento(input As Controllo_Inserimento_Dose_Prodotto, gridDosiManaged As List(Of RowGridDosi), Blocca_Inserimento As String, ByRef ListaErroriTemp As List(Of ErroreGias), objParametri_Utenti As AgronicaCoreParametri)

        Dim InfoOperazione As InfoOperazione = GetInfoOperazione(input.lavorazione.primaryKey.codice, tipoAttivita:=1) 'RR: Ho fissato tipoAttivita a QDC perchè non si controlla serve sapere solo operazione("Trattamento..")

        If InfoOperazione.IsTrattamento AndAlso InfoOperazione.controlloPolverulenti Then

            Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim blocca_mix_polverulento As Boolean = False
            If (objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_MIX_POLVERULENTI_NONPOLVERULENTI, objParametri_Utenti.UtenteUsername, objParametri_Utenti)) = "1" Then
                blocca_mix_polverulento = True
            End If

            If blocca_mix_polverulento Then

                Dim countPolverulento As Integer = 0
                Dim countNonPolverulento As Integer = 0
                For Each prodottoGriglia In gridDosiManaged

                    Select Case prodottoGriglia.Polverulento

                        Case Tipo_Polverulento.Polverulento
                            countPolverulento += 1

                        Case Tipo_Polverulento.NonPolverulento
                            countNonPolverulento += 1

                    End Select

                Next

                Dim messaggio = ""
                Select Case input.dettaglioTrattamento.polverulento

                    Case Tipo_Polverulento.Polverulento
                        If countNonPolverulento > 0 Then
                            Select Case Blocca_Inserimento
                                Case "1" 'blocco
                                    messaggio = My.Resources.AgronicaCoreMapper.ProdottoPolverulentoConNonPolverulentiBloccante
                                Case "2" 'avviso
                                    messaggio = My.Resources.AgronicaCoreMapper.ProdottoPolverulentoConNonPolverulentiWarning
                            End Select

                        End If

                    Case Tipo_Polverulento.NonPolverulento
                        If countPolverulento > 0 Then
                            Select Case Blocca_Inserimento
                                Case "1" 'blocco
                                    messaggio = My.Resources.AgronicaCoreMapper.ProdottoNonPolverulentoConPolverulentiBloccante
                                Case "2" 'avviso
                                    messaggio = My.Resources.AgronicaCoreMapper.ProdottoNonPolverulentoConPolverulentiWarning
                            End Select

                        End If
                End Select

                If Not String.IsNullOrEmpty(messaggio) Then
                    ListaErroriTemp.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", messaggio, ""))
                End If

            End If

        End If
    End Sub

    Private Sub CheckDoseMassima(doseEtichetta As DoseEtichetta, udm As UnitaDiMisura, DoseConsentitaDiserbo As Decimal, doseHa As Decimal, doseHl As Decimal, doseQ As Decimal, Blocca_Inserimento As String, ByRef errori As Dictionary(Of TipoErrore, List(Of String)), objParametri_Utenti As AgronicaCoreParametri)

        If doseEtichetta.DoseMax <> 0 Then

            Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim blocca_dose_massima As Boolean = False
            If (objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA, objParametri_Utenti.UtenteUsername, objParametri_Utenti)) = "1" Then
                blocca_dose_massima = True
            End If

            If blocca_dose_massima Then
                Dim doseMaxHa_k_L As Decimal
                Dim perHaHl As Integer = 0
                Dim Udm_Cod_Max_scomposta As Integer = 0
                Dim Udm_Cod_Trasformato As Integer = 0
                Dim MoltiplicatoreDose As Decimal = 1

                Dim objUdm As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                objUdm.ScomponiUdm(Udm_Cod_Max_scomposta, perHaHl, If(doseEtichetta.Udm IsNot Nothing, doseEtichetta.Udm.codice, 0), MoltiplicatoreDose)

                'converto to kg o litri
                Dim doseIndicata, Moltiplicatore As Decimal
                AgronicaCoreMetaSchemaDAL.UnitaMisura_R.ConvertiToKG_L(Udm_Cod_Max_scomposta, Udm_Cod_Trasformato, Moltiplicatore)
                doseMaxHa_k_L = doseEtichetta.DoseMax * Moltiplicatore * MoltiplicatoreDose
                AgronicaCoreMetaSchemaDAL.UnitaMisura_R.ConvertiToKG_L(udm.codice, Udm_Cod_Trasformato, Moltiplicatore)
                Select Case perHaHl
                    Case TipiEnumerativi.enum_UnitaMisura.Ettolitro
                        doseIndicata = doseHl * Moltiplicatore
                    Case TipiEnumerativi.enum_UnitaMisura.Ettaro
                        doseIndicata = doseHa * Moltiplicatore
                    Case TipiEnumerativi.enum_UnitaMisura.Quintali
                        doseIndicata = doseQ * Moltiplicatore
                    Case TipiEnumerativi.enum_UnitaMisura.Tonnellate
                        doseIndicata = doseQ * Moltiplicatore * 10
                End Select

                If doseIndicata > doseMaxHa_k_L Then

                    Dim messaggio As String = ""
                    Select Case perHaHl
                        Case TipiEnumerativi.enum_UnitaMisura.Ettolitro
                            messaggio = My.Resources.AgronicaCoreMapper.DoseHlSelezionataNonPuoSup
                        Case TipiEnumerativi.enum_UnitaMisura.Ettaro
                            messaggio = My.Resources.AgronicaCoreMapper.DoseHaSelezionataNonPuoSup
                    End Select

                    Select Case Blocca_Inserimento
                        Case "1" 'blocco
                            errori(TipoErrore.BloccantePerImpostazioneUtente).Add(messaggio)
                        Case "2" 'avviso
                            errori(TipoErrore.NonBloccantePerImpostazioneUtente).Add(messaggio)
                    End Select

                End If

                If DoseConsentitaDiserbo <> 0 Then
                    If doseIndicata > DoseConsentitaDiserbo Then
                        Dim messaggio As String = String.Format(My.Resources.AgronicaCoreMapper.DoseDiserboNonRispettata, DoseConsentitaDiserbo)
                        Select Case Blocca_Inserimento
                            Case "1" 'blocco
                                errori(TipoErrore.BloccantePerImpostazioneUtente).Add(messaggio)
                            Case "2" 'avviso
                                errori(TipoErrore.NonBloccantePerImpostazioneUtente).Add(messaggio)
                        End Select
                    End If
                End If

            End If
        End If

    End Sub

    Private Sub CheckDoseMinima(doseEtichetta As DoseEtichetta, udm As UnitaDiMisura, doseHa As Decimal, doseHl As Decimal, doseQ As Decimal, Blocca_Inserimento As String, ByRef errori As Dictionary(Of TipoErrore, List(Of String)), objParametri_Utenti As AgronicaCoreParametri)

        If doseEtichetta.DoseMin <> 0 Then

            Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim blocca_dose_minima As Boolean = False
            If (objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA, objParametri_Utenti.UtenteUsername, objParametri_Utenti)) = "1" Then
                blocca_dose_minima = True
            End If

            If blocca_dose_minima Then
                Dim doseMinHa_k_L As Decimal
                Dim perHaHl As Integer = 0
                Dim Udm_Cod_Max_scomposta As Integer = 0
                Dim Udm_Cod_Trasformato As Integer = 0
                Dim MoltiplicatoreDose As Decimal = 1

                Dim objUdm As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                objUdm.ScomponiUdm(Udm_Cod_Max_scomposta, perHaHl, If(doseEtichetta.Udm IsNot Nothing, doseEtichetta.Udm.codice, 0), MoltiplicatoreDose)

                'converto to kg o litri
                Dim doseIndicata, Moltiplicatore As Decimal
                AgronicaCoreMetaSchemaDAL.UnitaMisura_R.ConvertiToKG_L(Udm_Cod_Max_scomposta, Udm_Cod_Trasformato, Moltiplicatore)
                doseMinHa_k_L = doseEtichetta.DoseMin * Moltiplicatore * MoltiplicatoreDose
                AgronicaCoreMetaSchemaDAL.UnitaMisura_R.ConvertiToKG_L(udm.codice, Udm_Cod_Trasformato, Moltiplicatore)
                Select Case perHaHl
                    Case TipiEnumerativi.enum_UnitaMisura.Ettolitro
                        doseIndicata = doseHl * Moltiplicatore
                    Case TipiEnumerativi.enum_UnitaMisura.Ettaro
                        doseIndicata = doseHa * Moltiplicatore
                    Case TipiEnumerativi.enum_UnitaMisura.Quintali
                        doseIndicata = doseQ * Moltiplicatore
                    Case TipiEnumerativi.enum_UnitaMisura.Tonnellate
                        doseIndicata = doseQ * Moltiplicatore * 10
                End Select

                If doseIndicata < doseMinHa_k_L Then

                    Dim messaggio As String = ""
                    Select Case perHaHl
                        Case TipiEnumerativi.enum_UnitaMisura.Ettolitro
                            messaggio = My.Resources.AgronicaCoreMapper.DoseHlSelezionataNonPuoInf
                        Case TipiEnumerativi.enum_UnitaMisura.Ettaro
                            messaggio = My.Resources.AgronicaCoreMapper.DoseHaSelezionataNonPuoInf
                    End Select

                    Select Case Blocca_Inserimento
                        Case "1" 'blocco
                            errori(TipoErrore.BloccantePerImpostazioneUtente).Add(messaggio)
                        Case "2" 'avviso
                            errori(TipoErrore.NonBloccantePerImpostazioneUtente).Add(messaggio)
                    End Select

                End If
            End If
        End If

    End Sub

    Private Sub CheckNumeroMaxTrattamenti(doseEtichetta As DoseEtichetta, input As Controllo_Inserimento_Dose_Prodotto, Blocca_Inserimento As String, ByRef errori As Dictionary(Of TipoErrore, List(Of String)), objParametri_Utenti As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri)

        If doseEtichetta.Limite > 0 Then

            Dim Blocca_Numero_Massimo As Boolean = False
            Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            If (objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA, objParametri_Utenti.UtenteUsername, objParametri_Utenti)) = "1" Then
                Blocca_Numero_Massimo = True
            End If

            If Blocca_Numero_Massimo Then

                'ciclo sugli impianti selezionati in modo da identificare piva,sa_cod,Appezza,Id_reg
                Dim Str_FiltroAggiuntivo As New StringBuilder
                Str_FiltroAggiuntivo.Append("(")
                For i = 0 To input.row_grid_impianti.Count - 1
                    Str_FiltroAggiuntivo.Append("( ")
                    Str_FiltroAggiuntivo.Append("Mov_Destinazioni.Piva = " & Agro_SQL_SaveText_NULL(input.row_grid_impianti(i).PIVA) & " ")
                    Str_FiltroAggiuntivo.Append("AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(input.row_grid_impianti(i).SA_COD) & " ")
                    Str_FiltroAggiuntivo.Append("AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(input.row_grid_impianti(i).APPEZZA) & " ")
                    Str_FiltroAggiuntivo.Append("AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(input.row_grid_impianti(i).ID_REG) & " ")
                    Str_FiltroAggiuntivo.Append(" ) ")
                    If i < input.row_grid_impianti.Count - 1 Then
                        Str_FiltroAggiuntivo.Append(" OR ")
                    Else
                        Str_FiltroAggiuntivo.Append(" ) ")
                    End If
                Next
                Str_FiltroAggiuntivo.Append(" AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(input.Data) & " AND Imprese_Progetti.Validita_Fine  >= " & Agro_SQL_SaveDate(input.Data) & " ")

                Dim listaAgendeMulticentro = Utility.GetAgendeMulticentroFromParametri(input.parametri_aggiuntivi_list)
                If listaAgendeMulticentro IsNot Nothing AndAlso listaAgendeMulticentro.Count > 0 Then
                    For Each agendaMulticentro In listaAgendeMulticentro
                        Str_FiltroAggiuntivo.Append(" AND Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(agendaMulticentro) & " ")
                    Next
                Else
                    If input.id_agenda <> 0 Then
                        Str_FiltroAggiuntivo.Append(" AND Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(input.id_agenda) & " ")
                    End If
                End If


                Dim dt_Movimenti As DataTable
                Dim objMovimentiDettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                dt_Movimenti = objMovimentiDettagli.NumeroTrattamentiperImpianti(input.dettaglioTrattamento.prodotto.codice, Str_FiltroAggiuntivo.ToString, "", objParametri_Server)

                Dim Dt_Movimenti_SingolaDose As DataTable
                Str_FiltroAggiuntivo.Append(" AND (Movimenti_dettagli.DoseEtichetta_Value like '" & Agro_SQL_SaveText(doseEtichetta.codice.ToString) & "$%' ")
                Str_FiltroAggiuntivo.Append("      OR  Movimenti_dettagli.DoseEtichetta_Value like '<br>" & Agro_SQL_SaveText(doseEtichetta.codice.ToString) & "$%') ")
                Dt_Movimenti_SingolaDose = objMovimentiDettagli.NumeroTrattamentiperImpianti(input.dettaglioTrattamento.prodotto.codice, Str_FiltroAggiuntivo.ToString, "", objParametri_Server)

                Dim str_Controllo_Aggiuntivo As String = ""

                '(26/04/2017 fede) modificato controllo 
                'verranno controllate le seguenti unità di misura (2019=anno, 2024=stagione colturale, 2025=ciclo colturale) 
                'e verranno controllate sempre in base alle date dell'esercizio

                Dim dr() As DataRow
                Dim dr_s() As DataRow
                If doseEtichetta.UdmLimite IsNot Nothing Then
                    Select Case doseEtichetta.UdmLimite.codice
                        Case TipiEnumerativi.enum_UnitaMisura.CicloColturale, TipiEnumerativi.enum_UnitaMisura.StagioneColturale, TipiEnumerativi.enum_UnitaMisura.Anno
                            'distinta
                            str_Controllo_Aggiuntivo = " AND Data_Movimento >= Validita_Inizio AND Data_Movimento <= Validita_Fine "
                    End Select
                End If

                Dim str_Errore As String = ""
                Dim str_Errore_Globale As String = ""
                For i = 0 To input.row_grid_impianti.Count - 1

                    dr = dt_Movimenti.Select(" Piva ='" & input.row_grid_impianti(i).PIVA & "'" &
                                                  " AND Sa_Cod =" & input.row_grid_impianti(i).SA_COD &
                                                  " AND Appezza =" & input.row_grid_impianti(i).APPEZZA &
                                                  " AND ID_Destinazione =" & input.row_grid_impianti(i).ID_REG &
                                                  str_Controllo_Aggiuntivo)

                    dr_s = Dt_Movimenti_SingolaDose.Select(" Piva ='" & input.row_grid_impianti(i).PIVA & "'" &
                                                              " AND Sa_Cod =" & input.row_grid_impianti(i).SA_COD &
                                                              " AND Appezza =" & input.row_grid_impianti(i).APPEZZA &
                                                              " AND ID_Destinazione =" & input.row_grid_impianti(i).ID_REG &
                                                              str_Controllo_Aggiuntivo)

                    '(12/10/2018 fede) se ho il globale valorizzato
                    'verifico il limite solo sulla dose selezionata + il globale su tutti 
                    If doseEtichetta.Num_Max_Interventi_Globali > 0 Then

                        If dr_s.Length >= doseEtichetta.Limite Then
                            str_Errore = str_Errore + " (" & input.row_grid_impianti(i).APP_NOME & ")"
                        End If

                        If dr.Length >= doseEtichetta.Num_Max_Interventi_Globali Then
                            str_Errore_Globale = str_Errore_Globale + " (" & input.row_grid_impianti(i).APP_NOME & ")"
                        End If

                    Else

                        'caso vecchio
                        If dr.Length >= doseEtichetta.Limite Then
                            str_Errore = str_Errore + " (" & input.row_grid_impianti(i).APP_NOME & ")"
                        End If

                    End If

                Next

                If str_Errore <> "" Or str_Errore_Globale <> "" Then

                    Dim messaggio As String = ""

                    If str_Errore <> "" Then
                        messaggio &= String.Format(My.Resources.AgronicaCoreMapper.IlNumeroMassimoDiX0TrattamentiX1IndicatiDa, doseEtichetta.Limite, If(doseEtichetta.UdmLimite IsNot Nothing, doseEtichetta.UdmLimite.simbolo, "")) & str_Errore
                    End If

                    If str_Errore_Globale <> "" Then
                        messaggio &= String.Format(My.Resources.AgronicaCoreMapper.IlNumeroMassimoDiX0TrattamentiX1IndicatiDa, doseEtichetta.Num_Max_Interventi_Globali, If(doseEtichetta.UdmLimite IsNot Nothing, doseEtichetta.UdmLimite.simbolo, "")) & str_Errore_Globale
                    End If

                    Select Case Blocca_Inserimento
                        Case "1" 'blocco
                            errori(TipoErrore.BloccantePerImpostazioneUtente).Add(messaggio)
                        Case "2" 'avviso
                            errori(TipoErrore.NonBloccantePerImpostazioneUtente).Add(messaggio)
                    End Select

                End If
            End If
        End If

    End Sub

    Private Sub CheckIntervalloTrattamenti(doseEtichetta As DoseEtichetta, input As Controllo_Inserimento_Dose_Prodotto, Blocca_Inserimento As String, ByRef errori As Dictionary(Of TipoErrore, List(Of String)), objParametri_Utenti As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri)

        If doseEtichetta.IntervalloTrattamenti_Min > 0 Then

            Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim Blocca_Intervallo_Minimo As Boolean = False
            If (objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA, objParametri_Utenti.UtenteUsername, objParametri_Utenti)) = "1" Then
                Blocca_Intervallo_Minimo = True
            End If

            If Blocca_Intervallo_Minimo Then

                'ciclo sugli impianti selezionati in modo da identificare piva,sa_cod,Appezza,Id_reg
                Dim Str_FiltroAggiuntivo As New StringBuilder
                Str_FiltroAggiuntivo.Append("(")
                For i = 0 To input.row_grid_impianti.Count - 1
                    Str_FiltroAggiuntivo.Append("( ")
                    Str_FiltroAggiuntivo.Append("Mov_Destinazioni.Piva = " & Agro_SQL_SaveText_NULL(input.row_grid_impianti(i).PIVA) & " ")
                    Str_FiltroAggiuntivo.Append("AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(input.row_grid_impianti(i).SA_COD) & " ")
                    Str_FiltroAggiuntivo.Append("AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(input.row_grid_impianti(i).APPEZZA) & " ")
                    Str_FiltroAggiuntivo.Append("AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(input.row_grid_impianti(i).ID_REG) & " ")
                    Str_FiltroAggiuntivo.Append(" ) ")
                    If i < input.row_grid_impianti.Count - 1 Then
                        Str_FiltroAggiuntivo.Append(" OR ")
                    Else
                        Str_FiltroAggiuntivo.Append(" ) ")
                    End If
                Next
                Str_FiltroAggiuntivo.Append(" AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(input.Data) & " AND Imprese_Progetti.Validita_Fine  >= " & Agro_SQL_SaveDate(input.Data) & " ")

                Dim listaAgendeMulticentro = Utility.GetAgendeMulticentroFromParametri(input.parametri_aggiuntivi_list)
                If listaAgendeMulticentro IsNot Nothing AndAlso listaAgendeMulticentro.Count > 0 Then
                    For Each agendaMulticentro In listaAgendeMulticentro
                        Str_FiltroAggiuntivo.Append(" AND Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(agendaMulticentro) & " ")
                    Next
                Else
                    If input.id_agenda <> 0 Then
                        Str_FiltroAggiuntivo.Append(" AND Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(input.id_agenda) & " ")
                    End If
                End If

                Dim dt_Movimenti As DataTable
                Dim objMovimentiDettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                dt_Movimenti = objMovimentiDettagli.UltimoTrattamentoImpianti(input.dettaglioTrattamento.prodotto.codice, input.Data, Str_FiltroAggiuntivo.ToString, " data_movimento desc", objParametri_Server)

                Dim strErroreMin As String = ""

                'ciclo per ogni impianto
                For i = 0 To input.row_grid_impianti.Count - 1
                    Dim dr() As DataRow = dt_Movimenti.Select(" Piva ='" & input.row_grid_impianti(i).PIVA & "'" &
                                            " AND Sa_Cod =" & input.row_grid_impianti(i).SA_COD &
                                            " AND Appezza =" & input.row_grid_impianti(i).APPEZZA &
                                            " AND ID_Destinazione =" & input.row_grid_impianti(i).ID_REG)

                    'ho qualche trattamento passato
                    If dr.Length > 0 Then
                        Dim data As Date = dr(0).Item("data_movimento")

                        If data.AddDays(doseEtichetta.IntervalloTrattamenti_Min) > input.Data Then
                            'errore
                            If strErroreMin <> "" Then
                                strErroreMin &= " - "
                            End If
                            strErroreMin &= input.row_grid_impianti(i).APP_NOME
                        End If
                    End If
                Next

                If strErroreMin <> "" Then

                    Dim messaggio As String = ""

                    If strErroreMin <> "" Then
                        messaggio &= String.Format(My.Resources.AgronicaCoreMapper.IntervalloMinimoNonRispettato, strErroreMin, doseEtichetta.IntervalloTrattamenti_Min)
                    End If

                    Select Case Blocca_Inserimento
                        Case "1" 'blocco
                            errori(TipoErrore.BloccantePerImpostazioneUtente).Add(messaggio)
                        Case "2" 'avviso
                            errori(TipoErrore.NonBloccantePerImpostazioneUtente).Add(messaggio)
                    End Select

                End If
            End If
        End If

    End Sub

    Private Sub CheckAcqua(doseEtichetta As DoseEtichetta,
                           input As Controllo_Inserimento_Dose_Prodotto,
                           Blocca_Inserimento As String,
                           infoOperazione As InfoOperazione,
                           ByRef errori As Dictionary(Of TipoErrore, List(Of String)),
                           objParametri_Utenti As AgronicaCoreParametri,
                           objParametri_Server As AgronicaCoreParametri)

        If input.risorsaAcqua IsNot Nothing AndAlso doseEtichetta.UdmAcqua IsNot Nothing Then

            Dim Acqua_Min As Decimal = doseEtichetta.AcquaMin
            Dim Acqua_Max As Decimal = doseEtichetta.AcquaMax
            Dim AcquaUdm_Cod = If(doseEtichetta.UdmAcqua IsNot Nothing, doseEtichetta.UdmAcqua.codice, 0)

            'converto l'acqua in Hl
            Dim AcquaUdm_Radice, Acquaper_ha_hl As Integer
            AcquaUdm_Radice = -1
            Dim objUdm As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
            objUdm.ScomponiUdm(AcquaUdm_Radice, Acquaper_ha_hl, AcquaUdm_Cod)

            Dim AcquaMaxHL As Decimal = 0
            Dim AcquaMinHL As Decimal = 0

            If AcquaUdm_Radice <> -1 Then

                If Acquaper_ha_hl = TipiEnumerativi.enum_UnitaMisura.Ettaro Then
                    Select Case AcquaUdm_Radice
                        Case enum_UnitaMisura.KG
                            AcquaMaxHL = Acqua_Max / 100
                            AcquaMinHL = Acqua_Min / 100
                        Case enum_UnitaMisura.Quintali
                            AcquaMaxHL = Acqua_Max
                            AcquaMinHL = Acqua_Min
                        Case enum_UnitaMisura.Tonnellate
                            AcquaMaxHL = Acqua_Max * 10
                            AcquaMinHL = Acqua_Min * 10
                        Case enum_UnitaMisura.Grammi
                            AcquaMaxHL = Acqua_Max / 10000
                            AcquaMinHL = Acqua_Min / 10000
                        Case enum_UnitaMisura.Milligrammi
                            AcquaMaxHL = Acqua_Max / 100000
                            AcquaMinHL = Acqua_Min / 100000
                        Case enum_UnitaMisura.Litri
                            AcquaMaxHL = Acqua_Max / 100
                            AcquaMinHL = Acqua_Min / 100
                        Case enum_UnitaMisura.Millilitri
                            AcquaMaxHL = Acqua_Max / 100000
                            AcquaMinHL = Acqua_Min / 100000
                        Case enum_UnitaMisura.CentimetriCubi
                            AcquaMaxHL = Acqua_Max / 100000
                            AcquaMinHL = Acqua_Min / 100000
                    End Select

                End If
            End If

            Dim messaggio As String = ""

            Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

            Dim Dose_Acqua_Ha As Decimal = 0

            Dim qtaTrattata As Decimal = 0
            Select Case infoOperazione.TipoCentroDiCosto
                Case centri_di_costo.Tipo.ProdottoDaTrattare
                    qtaTrattata = input.qtaProdotto_Trattata
                Case Else
                    qtaTrattata = input.sup_Trattata
            End Select

            Select Case input.risorsaAcqua.doseAcqua
                Case RisorsaAcqua.TipoDoseAcqua.TOTALE
                    Dose_Acqua_Ha = input.risorsaAcqua.acqua / qtaTrattata
                Case RisorsaAcqua.TipoDoseAcqua.HA
                    Dose_Acqua_Ha = input.risorsaAcqua.acqua
            End Select

            'controllo che qta Max
            If Acqua_Max <> 0 Then

                Dim Blocca_Acqua_Massima As Boolean = False
                If (objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA, objParametri_Utenti.UtenteUsername, objParametri_Utenti)) = "1" Then
                    Blocca_Acqua_Massima = True
                End If

                If Blocca_Acqua_Massima Then

                    If Dose_Acqua_Ha > AcquaMaxHL Then

                        'entrambe le quantità sono già espresse in HL
                        messaggio &= My.Resources.AgronicaCoreMapper.QuantitaAcquaNonPuoSup

                        Select Case Blocca_Inserimento
                            Case "1" 'blocco
                                errori(TipoErrore.BloccantePerImpostazioneUtente).Add(messaggio)
                            Case "2" 'avviso
                                errori(TipoErrore.NonBloccantePerImpostazioneUtente).Add(messaggio)
                        End Select

                    End If
                End If
            End If

            'controllo che qta Min
            If Acqua_Min <> 0 Then

                Dim Blocca_Acqua_Minima As Boolean = False
                If (objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA, objParametri_Utenti.UtenteUsername, objParametri_Utenti)) = "1" Then
                    Blocca_Acqua_Minima = True
                End If

                If Blocca_Acqua_Minima Then

                    If Dose_Acqua_Ha < AcquaMinHL Then

                        'entrambe le quantità sono già espresse in HL
                        messaggio &= My.Resources.AgronicaCoreMapper.QuantitaAcquaNonPuoInf

                        Select Case Blocca_Inserimento
                            Case "1" 'blocco
                                errori(TipoErrore.BloccantePerImpostazioneUtente).Add(messaggio)
                            Case "2" 'avviso
                                errori(TipoErrore.NonBloccantePerImpostazioneUtente).Add(messaggio)
                        End Select

                    End If
                End If

            End If

        End If

    End Sub

    Private Function GetGruppoFinalita(ByVal impianti As List(Of RowGridImpianti), objParametri_Server As AgronicaCoreParametri) As Integer

        Dim lstImpianti As New List(Of Impianto)
        For Each impiantoGrid In impianti
            Dim centroPK = New CentroAziendale.PK(impiantoGrid.SA_COD, impiantoGrid.PIVA)
            Dim appezzamentoPK = New Appezzamento.PK(impiantoGrid.APPEZZA, centroPK)
            Dim impiantoPK = New Impianto.PK(impiantoGrid.ID_REG, appezzamentoPK)
            Dim impianto = New Impianto(impiantoPK)

            lstImpianti.Add(impianto)
        Next

        Return STD_Utility.IdentificaGrfi_Cod(lstImpianti.ToArray, objParametri_Server)

    End Function

#Region "Check Massimali"

    Private Sub EstraiSostanzeOperazione(
        ByRef N_Operazione As Decimal,
        ByRef P_Operazione As Decimal,
        ByRef K_Operazione As Decimal,
        ByRef Cu_Operazione As Decimal,
        input As Controllo_Inserimento_Dose_Prodotto,
        gridDosiManaged As List(Of RowGridDosi),
        ConsideraProdottoInInserimento As Boolean
    )
        Dim Dose_Ha_KGL As Decimal
        Dim Moltiplicatore As Decimal = 0
        Dim Udm_Cod_Trasformato As Integer = 0

        If ConsideraProdottoInInserimento AndAlso input.doseHa > 0 Then

            AgronicaCoreMetaSchemaDAL.UnitaMisura_R.ConvertiToKG_L(input.unitadiMisura.codice, Udm_Cod_Trasformato, Moltiplicatore)
            Dose_Ha_KGL = input.doseHa * Moltiplicatore

            If input.dettaglioFertilizzazione IsNot Nothing Then
                N_Operazione = Dose_Ha_KGL * input.N_Utile / 100
                P_Operazione = Dose_Ha_KGL * input.P / 100
                K_Operazione = Dose_Ha_KGL * input.K / 100
                Cu_Operazione = Dose_Ha_KGL * input.Cu / 100
            End If

            If input.dettaglioTrattamento IsNot Nothing Then
                Cu_Operazione += estraiRameTrattamento(input.dettaglioTrattamento.principiAttivi, Dose_Ha_KGL, input.unitadiMisura.codice)
            End If
        End If

        Dim TotCu_AltreGriglie As Decimal = 0
        If input.row_grid_dosi_altre_operazioni IsNot Nothing Then
            For Each row In input.row_grid_dosi_altre_operazioni
                Dim InfoOperazione As InfoOperazione = GetInfoOperazione(row.Operazione.primaryKey.codice, tipoAttivita:=1)
                AgronicaCoreMetaSchemaDAL.UnitaMisura_R.ConvertiToKG_L(row.UdM.codice, Udm_Cod_Trasformato, Moltiplicatore)
                Dose_Ha_KGL = row.Dose_Ha * Moltiplicatore

                Select Case InfoOperazione.classType
                    Case AgronicaCoreModelsSTD.costanti.ClassType.DettaglioFertilizzazione
                        TotCu_AltreGriglie += estraiRameFertilizzazione(row)
                    Case AgronicaCoreModelsSTD.costanti.ClassType.DettaglioTrattamento
                        TotCu_AltreGriglie += estraiRameTrattamento(row.PrincipiAttivi, Dose_Ha_KGL, row.UdM.codice)
                End Select
            Next

            Cu_Operazione += TotCu_AltreGriglie
        End If

        'Estraggo il rame della riga corrente (messo qui sotto perchè ho bisogno della superficie)

        For Each prodottoGriglia In gridDosiManaged
            AgronicaCoreMetaSchemaDAL.UnitaMisura_R.ConvertiToKG_L(prodottoGriglia.UdM.codice, Udm_Cod_Trasformato, Moltiplicatore)
            Dose_Ha_KGL = prodottoGriglia.Dose_Ha * Moltiplicatore

            If input.dettaglioFertilizzazione IsNot Nothing Then
                N_Operazione += Dose_Ha_KGL * prodottoGriglia.N_Utile / 100
                P_Operazione += Dose_Ha_KGL * prodottoGriglia.P / 100
                K_Operazione += Dose_Ha_KGL * prodottoGriglia.K / 100
                Cu_Operazione += Dose_Ha_KGL * prodottoGriglia.Cu / 100
            End If

            If input.dettaglioTrattamento IsNot Nothing Then
                Cu_Operazione += estraiRameTrattamento(prodottoGriglia.PrincipiAttivi, Dose_Ha_KGL, prodottoGriglia.UdM.codice)
            End If
        Next
    End Sub

    Private Sub GestioneRegolamentoSpecieMassimale(
        ByRef N_Max_Intervento As Decimal,
        Veg_Cod As Integer, input As Controllo_Inserimento_Dose_Prodotto,
        objParametri_Server As AgronicaCoreParametri, objParametri_Super_Server As AgronicaCoreParametri
    )
        'DT: per ora non è previsto che ci possano essere più fertilizzazioni nella stessa multi-operazione.
        'se in futuro fosse da gestire, bisogna controllare i massimali sommando tutte le fertilizzazioni
        Dim Regolamento_Cod As Integer = input.disciplinare.regolamentoConcimazione.codice

        If Regolamento_Cod > 0 AndAlso Veg_Cod > 0 Then
            Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output
            Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input With {
                .Regolamento_Cod = Regolamento_Cod,
                .Veg_Cod = Veg_Cod,
                .Grfi_Cod = 0
            }

            '(26/03/2020 fede) aggiunta finalita gias
            '(grfi_cod è la finalita_rer)
            Dim Grfi_cod_Gias As Integer = GetGruppoFinalita(input.row_grid_impianti, objParametri_Server)
            objParametriIngresso.Grfi_Cod_Gias = Grfi_cod_Gias

            objParametriIngresso.SoloValorizzati = True
            objParametriIngresso.SoloVisibili = False
            objParametriIngresso.Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.N_Max_Intervento
            Dim agroWs As String
            Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
            If agroWs = "" Then
                agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Super_Server)
            End If
            objParametriIngresso.url = agroWs
            Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
            objParametriUscita = objPC_WS.FattoriCorrettivi_ConFinalitaGias_Leggi(objParametriIngresso)
            If objParametriUscita.ListaFattoriCorrettivi.Count > 0 Then
                N_Max_Intervento = objParametriUscita.ListaFattoriCorrettivi.Item(0).Valore
            End If
        End If
    End Sub

    Private Function CalcolaRameDistribuito(
        impianto As RowGridImpianti,
        input As Controllo_Inserimento_Dose_Prodotto,
        ByRef Hash_FormulatiPA As Hashtable,
        ByRef Hash_FormulatiPAPesi As Hashtable,
        objParametri_Server As AgronicaCoreParametri,
        objParametri_Utenti As AgronicaCoreParametri
    )
        Dim dtLavorazioni As DataTable
        Dim ObjContab As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R
        Dim xFiltroAggiuntivo As String = ""
        If input.id_agenda <> 0 Then
            xFiltroAggiuntivo += "Agenda.ID_Agenda <> " & input.id_agenda & " "
            If input.raccoglitore_cod <> 0 Then
                xFiltroAggiuntivo += "AND ISNULL(Agenda.Raccoglitore_Cod, 0) <> " & input.raccoglitore_cod
            End If
        End If

        dtLavorazioni = ObjContab.LeggiLavorazioni_Da_Principi_Attivi(
                impianto.PIVA, impianto.SA_COD, PaRameici_str,
                impianto.APPEZZA, impianto.ID_REG,
                impianto.Validita_Inizio_Distinta, impianto.Validita_Fine_Distinta,
                xFiltroAggiuntivo, "", objParametri_Server, objParametri_Utenti, Hash_FormulatiPA, Hash_FormulatiPAPesi)

        Dim Qta_Pa_Tot As Decimal = 0
        Dim Qta_Pa_Ha As Decimal = 0
        Dim Qta_Prodotto_Ha As Decimal = 0
        Dim Sup As Decimal
        Dim TitoloP As Decimal
        Dim PesoP As Decimal

        For o = 0 To dtLavorazioni.Rows.Count - 1
            TitoloP = 0
            PesoP = 0
            If Not IsDBNull(dtLavorazioni.Rows(o).Item("titolo")) AndAlso CDec(dtLavorazioni.Rows(o).Item("titolo")) > 0 Then
                TitoloP = CDec(dtLavorazioni.Rows(o).Item("titolo"))
            End If
            If Not IsDBNull(dtLavorazioni.Rows(o).Item("peso")) AndAlso CDec(dtLavorazioni.Rows(o).Item("peso")) > 0 Then
                PesoP = CDec(dtLavorazioni.Rows(o).Item("peso"))
            End If
            If Not IsDBNull(dtLavorazioni.Rows(o).Item("sup_trattata")) AndAlso CDbl(dtLavorazioni.Rows(o).Item("sup_trattata")) > 0 Then
                Sup = CDbl(dtLavorazioni.Rows(o).Item("sup_trattata"))
            Else
                Sup = CDbl(dtLavorazioni.Rows(o).Item("sup_imp"))
            End If
            If Sup <> 0 Then
                Qta_Prodotto_Ha = CDbl(dtLavorazioni.Rows(o).Item("qta")) / Sup
                If PesoP = 0 Then
                    Qta_Pa_Ha = Qta_Prodotto_Ha * TitoloP / 100
                Else
                    Qta_Pa_Ha = Qta_Prodotto_Ha * PesoP / 1000
                End If
                Qta_Pa_Tot += Qta_Pa_Ha
            End If
        Next

        Return Qta_Pa_Tot
    End Function

    Private Sub ValorizeError(
        N As MacroelementData,
        P As MacroelementData,
        K As MacroelementData,
        Cu As MacroelementData,
        ByRef nImpiantiViolati_N_Max_Intervento As Integer,
        ByRef Qta_Ha_Distribuibile_Prodotto As Decimal,
        impianto As RowGridImpianti,
        input As Controllo_Inserimento_Dose_Prodotto
    )
        If N.Massimo <> -1 Then
            If N.EccedeMassimale() AndAlso N.InOperazione > 0 Then
                N.StrError &= String.Format(My.Resources.AgronicaCoreMapper.LimiteKgHaMassimoNonRispettato, impianto.APP_NOME, (N.Distribuito + N.InOperazione), N.Massimo) + NEWLINE
                nImpiantiViolati_N_Max_Intervento += 1
            End If
            If input.N_Utile > 0 Then
                Qta_Ha_Distribuibile_Prodotto = IIf(Qta_Ha_Distribuibile_Prodotto > ((N.Massimo - N.Distribuito - N.InOperazione) / (input.N_Utile / 100)), Format((N.Massimo - N.Distribuito - N.InOperazione) / (input.N_Utile / 100), "###,###.00#"), Qta_Ha_Distribuibile_Prodotto)
            End If
        End If
        If P.Massimo <> -1 Then
            If P.EccedeMassimale() AndAlso P.InOperazione > 0 Then
                P.StrError &= String.Format(My.Resources.AgronicaCoreMapper.LimiteKgHaMassimoNonRispettato, impianto.APP_NOME, (P.Distribuito + P.InOperazione), P.Massimo) + NEWLINE
            End If
            If input.P > 0 Then
                Qta_Ha_Distribuibile_Prodotto = IIf(Qta_Ha_Distribuibile_Prodotto > ((P.Massimo - P.Distribuito - P.InOperazione) / (input.P / 100)), Format((P.Massimo - P.Distribuito - P.InOperazione) / (input.P / 100), "###,###.00#"), Qta_Ha_Distribuibile_Prodotto)
            End If
        End If
        If K.Massimo <> -1 Then
            If K.EccedeMassimale() AndAlso K.InOperazione > 0 Then
                K.StrError &= String.Format(My.Resources.AgronicaCoreMapper.LimiteKgHaMassimoNonRispettato, impianto.APP_NOME, (K.Distribuito + K.InOperazione), K.Massimo) + NEWLINE
            End If
            If input.K > 0 Then
                Qta_Ha_Distribuibile_Prodotto = IIf(Qta_Ha_Distribuibile_Prodotto > ((K.Massimo - K.Distribuito - K.InOperazione) / (input.K / 100)), Format((K.Massimo - K.Distribuito - K.InOperazione) / (input.K / 100), "###,###.00#"), Qta_Ha_Distribuibile_Prodotto)
            End If
        End If
        If Cu.Massimo <> -1 Then
            If Cu.EccedeMassimale() AndAlso Cu.InOperazione > 0 Then
                Cu.StrError &= String.Format(My.Resources.AgronicaCoreMapper.LimiteKgHaMassimoNonRispettato, impianto.APP_NOME, Math.Round((Cu.Distribuito + Cu.InOperazione), 3), Cu.Massimo) + NEWLINE
            End If
            If input.Cu > 0 Then
                Qta_Ha_Distribuibile_Prodotto = IIf(Qta_Ha_Distribuibile_Prodotto > ((Cu.Massimo - Cu.Distribuito - Cu.InOperazione) / (input.Cu / 100)), Format((Cu.Massimo - Cu.Distribuito - Cu.InOperazione) / (input.Cu / 100), "###,###.00#"), Qta_Ha_Distribuibile_Prodotto)
            End If
        End If
    End Sub

    Private Sub ValorizeRecipeError(
        N As MacroelementData,
        P As MacroelementData,
        K As MacroelementData,
        Cu As MacroelementData,
        Mg As MacroelementData,
        ByRef Qta_Ha_Distribuibile_Prodotto_Ricette As Decimal,
        impianto As RowGridImpianti,
        input As Controllo_Inserimento_Dose_Prodotto,
        objParametri_Server As AgronicaCoreParametri
    )
        '(09/06/2019 fede) aggiunto controllo su ricette
        Dim objDettRic As New AgronicaCoreContabDAL.Ricette_Dettagli_R
        objDettRic.Leggi_Macroelementi_Distribuiti(
                    N.Distribuito_Ricetta, P.Distribuito_Ricetta, K.Distribuito_Ricetta, Mg.Distribuito_Ricetta,
                    Cu.Distribuito_Ricetta, 0, 0, 0, 0, 0,
                    impianto.PIVA, impianto.SA_COD, impianto.APPEZZA, impianto.ID_REG, impianto.Progetto_Cod,
                    impianto.Validita_Inizio_Distinta, impianto.Validita_Fine_Distinta,
                    input.ricetta_operazione_cod, objParametri_Server, Raccoglitore_Cod_Escluso:=input.raccoglitore_cod)

        If N.Massimo <> -1 Then
            If N.EccedeMassimaleRicetta() AndAlso N.InOperazione > 0 Then
                N.StrErrorRecipe &= String.Format(My.Resources.AgronicaCoreMapper.LimiteKgHaMassimoNonRispettato, impianto.APP_NOME, (N.Distribuito_Ricetta + N.InOperazione), N.Massimo)
            End If
            If input.N_Utile > 0 Then
                Qta_Ha_Distribuibile_Prodotto_Ricette = IIf(Qta_Ha_Distribuibile_Prodotto_Ricette > ((N.Massimo - N.Distribuito_Ricetta - N.InOperazione) / (input.N_Utile / 100)), Format((N.Massimo - N.Distribuito_Ricetta - N.InOperazione) / (input.N_Utile / 100), "###,###.00#"), Qta_Ha_Distribuibile_Prodotto_Ricette)
            End If
        End If
        If P.Massimo <> -1 Then
            If P.EccedeMassimaleRicetta() AndAlso P.InOperazione > 0 Then
                P.StrErrorRecipe &= String.Format(My.Resources.AgronicaCoreMapper.LimiteKgHaMassimoNonRispettato, impianto.APP_NOME, (P.Distribuito_Ricetta + P.InOperazione), P.Massimo)
            End If
            If input.P > 0 Then
                Qta_Ha_Distribuibile_Prodotto_Ricette = IIf(Qta_Ha_Distribuibile_Prodotto_Ricette > ((P.Massimo - P.Distribuito_Ricetta - P.InOperazione) / (input.P / 100)), Format((P.Massimo - P.Distribuito_Ricetta - P.InOperazione) / (input.P / 100), "###,###.00#"), Qta_Ha_Distribuibile_Prodotto_Ricette)
            End If
        End If
        If K.Massimo <> -1 Then
            If K.EccedeMassimaleRicetta() AndAlso K.InOperazione > 0 Then
                K.StrErrorRecipe &= String.Format(My.Resources.AgronicaCoreMapper.LimiteKgHaMassimoNonRispettato, impianto.APP_NOME, (K.Distribuito_Ricetta + K.InOperazione), K.Massimo)
            End If
            If input.K > 0 Then
                Qta_Ha_Distribuibile_Prodotto_Ricette = IIf(Qta_Ha_Distribuibile_Prodotto_Ricette > ((K.Massimo - K.Distribuito_Ricetta - K.InOperazione) / (input.K / 100)), Format((K.Massimo - K.Distribuito_Ricetta - K.InOperazione) / (input.K / 100), "###,###.00#"), Qta_Ha_Distribuibile_Prodotto_Ricette)
            End If
        End If
        If Cu.Massimo <> -1 Then
            If Cu.EccedeMassimaleRicetta() AndAlso Cu.InOperazione > 0 Then
                Cu.StrErrorRecipe &= String.Format(My.Resources.AgronicaCoreMapper.LimiteKgHaMassimoNonRispettato, impianto.APP_NOME, (Cu.Distribuito_Ricetta + Cu.InOperazione), Cu.Massimo)
            End If
            If input.Cu > 0 Then
                Qta_Ha_Distribuibile_Prodotto_Ricette = IIf(Qta_Ha_Distribuibile_Prodotto_Ricette > ((Cu.Massimo - Cu.Distribuito_Ricetta - Cu.InOperazione) / (input.Cu / 100)), Format((Cu.Massimo - Cu.Distribuito_Ricetta - Cu.InOperazione) / (input.Cu / 100), "###,###.00#"), Qta_Ha_Distribuibile_Prodotto_Ricette)
            End If
        End If
    End Sub


    ''' <param name="input">Utilizzato per ricavare le informazioni sull'operazione e decidere quale impostazione controllare per il blocco sul rame</param>
    Private Function GerStrErrMassimali(
        N As MacroelementData,
        P As MacroelementData,
        K As MacroelementData,
        Cu As MacroelementData,
        input As Controllo_Inserimento_Dose_Prodotto,
        objParametri_Utenti As AgronicaCoreParametri
    )
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim infoOperazione As InfoOperazione = GetInfoOperazione(input.lavorazione.getCodice(), input.tipoAttivita)

        Dim bloccaSeSuperaMassimaliNPKMg = objUtentiImpostazioni.LeggiValoreImpostazioneScalare(
                enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS,
                objParametri_Utenti.UtenteUsername, objParametri_Utenti) <> "0"
        Dim bloccaSeSuperaMassimaliCu_fert = infoOperazione.IsFertilizzazione AndAlso objUtentiImpostazioni.LeggiValoreImpostazioneScalare(
                enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_FERTI,
                objParametri_Utenti.UtenteUsername, objParametri_Utenti) <> "0"
        Dim bloccaSeSuperaMassimaliCu_Tratt = infoOperazione.IsTrattamento AndAlso objUtentiImpostazioni.LeggiValoreImpostazioneScalare(
                enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO,
                objParametri_Utenti.UtenteUsername, objParametri_Utenti) <> "0"


        If N.StrError <> "" Then
            N.StrError = String.Format(My.Resources.AgronicaCoreMapper.QuantitaAzotoInEccesso, NEWLINE + N.StrError) + NEWLINE
        End If
        If P.StrError <> "" Then
            P.StrError = String.Format(My.Resources.AgronicaCoreMapper.QuantitaFosforoInEccesso, NEWLINE + P.StrError) + NEWLINE
        End If
        If K.StrError <> "" Then
            K.StrError = String.Format(My.Resources.AgronicaCoreMapper.QuantitaPotassioInEccesso, NEWLINE + K.StrError) + NEWLINE
        End If
        If Cu.StrError <> "" Then
            Cu.StrError = String.Format(My.Resources.AgronicaCoreMapper.QuantitaRameInEccesso, NEWLINE + Cu.StrError)
        End If

        Dim strRet = ""
        If bloccaSeSuperaMassimaliNPKMg Then
            strRet &= N.StrError & P.StrError & K.StrError
        End If
        If bloccaSeSuperaMassimaliCu_fert OrElse bloccaSeSuperaMassimaliCu_Tratt Then
            strRet &= Cu.StrError
        End If

        Return strRet
    End Function

    Private Function GerStrErrMassimaliRicetta(
        N As MacroelementData,
        P As MacroelementData,
        K As MacroelementData,
        Cu As MacroelementData,
        input As Controllo_Inserimento_Dose_Prodotto,
        objParametri_Utenti As AgronicaCoreParametri
    )
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim infoOperazione As InfoOperazione = GetInfoOperazione(input.lavorazione.getCodice(), input.tipoAttivita)

        Dim bloccaSeSuperaMassimaliNPKMg = objUtentiImpostazioni.LeggiValoreImpostazioneScalare(
                enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS,
                objParametri_Utenti.UtenteUsername, objParametri_Utenti) <> "0"
        Dim bloccaSeSuperaMassimaliCu_fert = infoOperazione.IsFertilizzazione AndAlso objUtentiImpostazioni.LeggiValoreImpostazioneScalare(
                enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_FERTI,
                objParametri_Utenti.UtenteUsername, objParametri_Utenti) <> "0"
        Dim bloccaSeSuperaMassimaliCu_Tratt = infoOperazione.IsTrattamento AndAlso objUtentiImpostazioni.LeggiValoreImpostazioneScalare(
                enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO,
                objParametri_Utenti.UtenteUsername, objParametri_Utenti) <> "0"

        If N.StrErrorRecipe <> "" Then
            N.StrErrorRecipe = String.Format(My.Resources.AgronicaCoreMapper.QuantitaAzotoInEccesso, NEWLINE + N.StrErrorRecipe) + NEWLINE
        End If
        If P.StrErrorRecipe <> "" Then
            P.StrErrorRecipe = String.Format(My.Resources.AgronicaCoreMapper.QuantitaFosforoInEccesso, NEWLINE + P.StrErrorRecipe) + NEWLINE
        End If
        If K.StrErrorRecipe <> "" Then
            K.StrErrorRecipe = String.Format(My.Resources.AgronicaCoreMapper.QuantitaPotassioInEccesso, NEWLINE + K.StrErrorRecipe) + NEWLINE
        End If
        If Cu.StrErrorRecipe <> "" Then
            Cu.StrErrorRecipe = String.Format(My.Resources.AgronicaCoreMapper.QuantitaRameInEccesso, NEWLINE + Cu.StrErrorRecipe)
        End If

        Dim strRet = ""
        If bloccaSeSuperaMassimaliNPKMg Then
            strRet &= N.StrErrorRecipe & P.StrErrorRecipe & K.StrErrorRecipe
        End If
        If bloccaSeSuperaMassimaliCu_fert OrElse bloccaSeSuperaMassimaliCu_Tratt Then
            strRet &= Cu.StrErrorRecipe
        End If

        Return strRet
    End Function

#End Region

    Public Sub CheckMassimali(ConsideraProdottoInInserimento As Boolean,
                              input As Controllo_Inserimento_Dose_Prodotto,
                              gridDosiManaged As List(Of RowGridDosi),
                              Veg_Cod As Integer,
                              ByRef Qta_Ha_Distribuibile_Prodotto As Decimal,
                              ByRef Qta_Ha_Distribuibile_Prodotto_Ricette As Decimal,
                              ByRef errori As Dictionary(Of TipoErrore, List(Of String)),
                              objParametri_Super_Server As AgronicaCoreParametri,
                              objParametri_Server As AgronicaCoreParametri,
                              objParametri_Utenti As AgronicaCoreParametri
    )
        Dim N As New MacroelementData() With {.InOperazione = 0, .Massimo = -1, .Distribuito = 0, .Distribuito_Ricetta = 0}
        Dim P As New MacroelementData() With {.InOperazione = 0, .Massimo = -1, .Distribuito = 0, .Distribuito_Ricetta = 0}
        Dim K As New MacroelementData() With {.InOperazione = 0, .Massimo = -1, .Distribuito = 0, .Distribuito_Ricetta = 0}
        Dim Mg As New MacroelementData() With {.InOperazione = 0, .Massimo = -1, .Distribuito = 0, .Distribuito_Ricetta = 0}
        Dim Cu As New MacroelementData() With {.InOperazione = 0, .Massimo = -1, .Distribuito = 0, .Distribuito_Ricetta = 0}

        EstraiSostanzeOperazione(N.InOperazione, P.InOperazione, K.InOperazione, Cu.InOperazione, input, gridDosiManaged, ConsideraProdottoInInserimento)

        Dim nImpiantiViolati As Integer = 0
        Dim nImpiantiViolati_Ricetta As Integer = 0

        Dim nImpiantiViolati_N_Max_Intervento As Integer = 0
        Dim N_Max_Intervento As Decimal = 0
        GestioneRegolamentoSpecieMassimale(N_Max_Intervento, Veg_Cod, input, objParametri_Server, objParametri_Super_Server)

        'se è stato selezionato un disciplinare o il regolamento bio
        'AF: 05/23 -- IL CONTROLLO RAME SI FA A PRESCINDERE DAL REGOLAMENTO SELEZIONATO O MENO
        'If Regolamento_Cod > 0 OrElse Regolamento_Cod = -2 Then 
        If input.Data >= #1/1/2019# Then
            Cu.Massimo = 4
        Else
            Cu.Massimo = 6
        End If
        'End If

        Dim Hash_FormulatiPA As New Hashtable
        Dim Hash_FormulatiPAPesi As New Hashtable

        Dim strRet As String = ""
        Dim strErr_N_Intervento As String = ""

        For i = 0 To input.row_grid_impianti.Count - 1
            Dim impianto As RowGridImpianti = input.row_grid_impianti(i)
            Dim N_Massimo_Org As Decimal = -1

            'Lettura dei massimi apporti macroelementi
            N.Massimo = -1
            P.Massimo = -1
            K.Massimo = -1
            Mg.Massimo = -1

            Dim objApporti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim Regolamento_Concimazioni_Cod As Integer = 0

            objApporti.Leggi_Macroelementi_Impostati(
                impianto.PIVA, impianto.SA_COD, impianto.APPEZZA,
                impianto.ID_REG, impianto.Progetto_Cod,
                N.Massimo, N_Massimo_Org, P.Massimo, K.Massimo, Mg.Massimo, Regolamento_Concimazioni_Cod,
                objParametri_Server)

            If N_Max_Intervento <> 0 AndAlso (N.InOperazione - N_Max_Intervento) > 0.001 Then
                strErr_N_Intervento &= String.Format(
                    My.Resources.AgronicaCoreMapper.LimiteKgHaPerInterventoNonRispettato,
                    impianto.APP_NOME, N.InOperazione, N_Max_Intervento)

                nImpiantiViolati_N_Max_Intervento += 1
            End If

            N.Distribuito = 0
            P.Distribuito = 0
            K.Distribuito = 0
            Mg.Distribuito = 0
            Cu.Distribuito = 0

            Dim objDett As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            objDett.Leggi_Macroelementi_Distribuiti(
                N.Distribuito, P.Distribuito, K.Distribuito, Mg.Distribuito, Cu.Distribuito, 0, 0, 0, 0, 0,
                impianto.PIVA, impianto.SA_COD, impianto.APPEZZA,
                impianto.ID_REG, impianto.Progetto_Cod,
                impianto.Validita_Inizio_Distinta, impianto.Validita_Fine_Distinta,
                input.id_agenda, objParametri_Server, input.raccoglitore_cod)

            '(20/06/2017 fede) per rame considero anche quello dei trattamenti
            'AF: 05/23 -- IL CONTROLLO RAME SI FA A PRESCINDERE DAL REGOLAMENTO SELEZIONATO O MENO
            'If Regolamento_Cod > 0 Or Regolamento_Cod = -2 Then
            Cu.Distribuito += CalcolaRameDistribuito(impianto, input, Hash_FormulatiPA, Hash_FormulatiPAPesi, objParametri_Server, objParametri_Utenti)
            'End If

            ValorizeError(N, P, K, Cu, nImpiantiViolati_N_Max_Intervento, Qta_Ha_Distribuibile_Prodotto, impianto, input)

            If (N.Massimo <> -1 AndAlso N.InOperazione > 0 AndAlso N.EccedeMassimale()) OrElse
                (P.Massimo <> -1 AndAlso P.InOperazione > 0 AndAlso P.EccedeMassimale()) OrElse
                (K.Massimo <> -1 AndAlso K.InOperazione > 0 AndAlso K.EccedeMassimale()) OrElse
                (Cu.Massimo <> -1 AndAlso Cu.InOperazione > 0 AndAlso Cu.EccedeMassimale()) Then
                nImpiantiViolati += 1
            End If

            If input.tipoAttivita = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta AndAlso
               input.statoAttivita = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire Then

                ValorizeRecipeError(N, P, K, Cu, Mg, Qta_Ha_Distribuibile_Prodotto_Ricette, impianto, input, objParametri_Server)

                If (N.Massimo <> -1 AndAlso N.InOperazione > 0 AndAlso N.EccedeMassimaleRicetta()) OrElse
                    (P.Massimo <> -1 AndAlso P.InOperazione > 0 AndAlso P.EccedeMassimaleRicetta()) OrElse
                    (K.Massimo <> -1 AndAlso K.InOperazione > 0 AndAlso K.EccedeMassimaleRicetta()) OrElse
                    (Cu.Massimo <> -1 AndAlso Cu.InOperazione > 0 AndAlso Cu.EccedeMassimaleRicetta()) Then
                    nImpiantiViolati_Ricetta += 1
                End If
            End If

        Next

        If nImpiantiViolati > 0 Then
            strRet &= GerStrErrMassimali(N, P, K, Cu, input, objParametri_Utenti)
        End If

        If strErr_N_Intervento <> "" Then
            strErr_N_Intervento = String.Format(My.Resources.AgronicaCoreMapper.QuantitaAzotoInEccessoPerIntervento, strErr_N_Intervento) + NEWLINE
        End If

        If nImpiantiViolati_N_Max_Intervento > 0 Then
            strRet &= strErr_N_Intervento
        End If

        '(09/06/2019 fede) aggiunto controllo su ricette
        If input.tipoAttivita = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta AndAlso
           input.statoAttivita = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire AndAlso
           nImpiantiViolati_Ricetta > 0 Then

            strRet &= GerStrErrMassimaliRicetta(N, P, K, Cu, input, objParametri_Utenti)
        End If

        If strRet <> String.Empty Then
            Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim errorSeverity = objUtentiImpostazioni.LeggiValoreImpostazioneScalare(
                enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_SALVA_FERTILIZZAZIONI,
                objParametri_Utenti.UtenteUsername, objParametri_Utenti)

            If errorSeverity = "1" Then 'Blocco
                errori(TipoErrore.BloccantePerImpostazioneUtente).Add(strRet)

            ElseIf errorSeverity = "2" Then 'Avviso
                errori(TipoErrore.NonBloccantePerImpostazioneUtente).Add(strRet)

            End If
        End If
    End Sub

    Private Shared Function estraiRameTrattamento(PrincipiAttivi As List(Of PrincipioAttivo),
                                                  QTA_Prodotto_HA_Kg_L As Decimal,
                                                  UDMIndicata As Integer) As Decimal

        Dim CUxHa_Trattamento As Decimal = 0

        'Lista di tutte le sostanze che congono del rame
        Dim PA_Rameici As Integer() = {102, 338, 350, 368, 369, 370, 371, 372, 529, 616, 636, 692, 883}

        'Ciclo i princi attivi fino a trovare i rameici e mi calcolo la dose di rame
        For Each PA In PrincipiAttivi
            '102,338,350,368,369,370,371,372,529,616,636,692,883
            If PA_Rameici.Contains(PA.codice) Then

                Dim Udm_Cod_Trasformato As Integer
                Dim Moltiplicatore As Decimal

                'Conversione forzata Kg/L 
                AgronicaCoreMetaSchemaDAL.UnitaMisura_R.ConvertiToKG_L(UDMIndicata, Udm_Cod_Trasformato, Moltiplicatore)

                'Calcolo il CU e lo aggiungo ai totali (globali e per singolo LAV_COD)
                If PA.peso = 0 Then
                    CUxHa_Trattamento += CDec(QTA_Prodotto_HA_Kg_L * PA.titolo / 100)
                Else
                    CUxHa_Trattamento += CDec(QTA_Prodotto_HA_Kg_L * PA.peso / 1000)
                End If
            End If
        Next

        Return CUxHa_Trattamento

    End Function

    Private Shared Function estraiRameFertilizzazione(row_grid_dose As RowGridDosi) As Decimal

        Dim Qta_Prodotto_HA As Decimal = row_grid_dose.Dose_Ha
        Dim Udm_Cod_Trasformato As Integer
        Dim Moltiplicatore As Decimal

        'Conversione forzata Kg/L 
        AgronicaCoreMetaSchemaDAL.UnitaMisura_R.ConvertiToKG_L(row_grid_dose.UdM.codice, Udm_Cod_Trasformato, Moltiplicatore)

        'Trovo la QTA di prodotto utilizzata sul singolo impianto
        Dim QTA_Prodotto_HA_Kg_L As Decimal = Qta_Prodotto_HA * Moltiplicatore

        Return CDec(QTA_Prodotto_HA_Kg_L * row_grid_dose.Cu / 100)

    End Function

    ''' <summary>
    ''' Controlla le differenze tra il prodotto in ribaltamento e quello iniziale del brogliaccio
    ''' </summary>
    ''' <param name="input"></param>
    ''' <param name="infoOperazione"></param>
    ''' <param name="errori"></param>
    Private Sub ChecKProdottoInRibaltamento(ByVal input As Controllo_Inserimento_Dose_Prodotto,
                                            ByVal infoOperazione As InfoOperazione,
                                            ByVal objParametri_Server As AgronicaCoreParametri,
                                            ByRef errori As Dictionary(Of TipoErrore, List(Of String)))

        ClearErrori(errori)

        If input.tipoAttivita = Attivita.Tipo_Attivita.QuadernoDiCampagna AndAlso Not IsNothing(input.Original_Row_Value) Then

            If infoOperazione.IsTrattamento OrElse infoOperazione.IsFertilizzazione Then

                If input.Original_Row_Value.Fr_Cod > 0 Then

                    CompareDosaggiFromRicetta(input, objParametri_Server, errori)

                    If infoOperazione.IsFertilizzazione Then

                        Dim original_N As Decimal = input.Original_Row_Value.N

                        Dim new_N As Decimal = input.N

                        If original_N <> new_N Then
                            errori(TipoErrore.NonBloccante).Add(String.Format(My.Resources.AgronicaCoreMapper.NDiversoDa, original_N))
                        End If


                        Dim original_P As Decimal = input.Original_Row_Value.P

                        Dim new_P As Decimal = input.P

                        If original_P <> new_P Then
                            errori(TipoErrore.NonBloccante).Add(String.Format(My.Resources.AgronicaCoreMapper.P2O5DiversoDa, original_P))
                        End If


                        Dim original_K As Decimal = input.Original_Row_Value.K

                        Dim new_K As Decimal = input.K

                        If original_K <> new_K Then
                            errori(TipoErrore.NonBloccante).Add(String.Format(My.Resources.AgronicaCoreMapper.K2ODiversoDa, original_K))
                        End If


                        Dim original_Cu As Decimal = input.Original_Row_Value.Cu

                        Dim new_Cu As Decimal = input.Cu

                        If original_Cu <> new_Cu Then
                            errori(TipoErrore.NonBloccante).Add(String.Format(My.Resources.AgronicaCoreMapper.CuDiversoDa, original_Cu))
                        End If


                        Dim original_Efficienza As Decimal = input.Original_Row_Value.Efficienza

                        Dim new_Efficienza As Decimal = input.efficienza

                        If original_Efficienza <> new_Efficienza Then
                            errori(TipoErrore.NonBloccante).Add(String.Format(My.Resources.AgronicaCoreMapper.EfficienzaDiversaDa, original_Efficienza))
                        End If
                    End If

                End If

            End If

        End If

    End Sub

    ''' <summary>
    ''' Controlla le differenze di dosaggi tra il prodotto in ribaltamento e quello iniziale del brogliaccio
    ''' </summary>
    ''' <param name="input"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="errori"></param>
    Private Sub CompareDosaggiFromRicetta(ByVal input As Controllo_Inserimento_Dose_Prodotto,
                                            ByVal objParametri_Server As AgronicaCoreParametri,
                                            ByRef errori As Dictionary(Of TipoErrore, List(Of String)))

        Dim objUdm As New AgronicaControlli_2010.STD_UnitaDiMisura

        Dim original_udm_cod As Integer = 0

        Dim original_udm_sim As String = ""

        Dim original_tipo_controllo_cod As Integer = 0

        If Not IsNothing(input.Original_Row_Value.UdM) Then

            If input.Original_Row_Value.UdM.codice > 0 Then
                original_udm_cod = input.Original_Row_Value.UdM.codice
            End If

            If input.Original_Row_Value.UdM.simbolo <> "" Then
                original_udm_sim = input.Original_Row_Value.UdM.simbolo
            End If

            If Not IsNothing(input.Original_Row_Value.UdM.tipoControllo) AndAlso input.Original_Row_Value.UdM.tipoControllo.codice > 0 Then
                original_tipo_controllo_cod = input.Original_Row_Value.UdM.tipoControllo.codice
            Else
                original_tipo_controllo_cod = objUdm.LeggiTipoControllo(original_udm_cod, objParametri_Server).codice
            End If

        End If

        Dim new_udm_cod As Integer = 0

        Dim new_tipo_controllo_cod As Integer = 0

        If Not IsNothing(input.unitadiMisura) AndAlso input.unitadiMisura.codice > 0 Then
            new_udm_cod = input.unitadiMisura.codice

            If Not IsNothing(input.unitadiMisura.tipoControllo) AndAlso input.unitadiMisura.tipoControllo.codice > 0 Then
                new_tipo_controllo_cod = input.unitadiMisura.tipoControllo.codice
            Else
                new_tipo_controllo_cod = objUdm.LeggiTipoControllo(new_udm_cod, objParametri_Server).codice
            End If
        End If


        Dim new_doseTot As Decimal = input.quantitaTotale

        Dim new_doseHa As Decimal = input.doseHa

        Dim new_doseHl As Decimal = input.doseHl

        Dim original_doseTot_converted As Decimal = input.Original_Row_Value.DoseTot_Ha

        Dim original_doseHa_converted As Decimal = input.Original_Row_Value.Dose_Ha

        Dim original_doseHl_converted As Decimal = input.Original_Row_Value.Dose_Hl

        If original_udm_cod <> new_udm_cod Then

            'Replicato lo stesso calcolo che viene fatto su angular quando cambia l'unità di misura

            Dim sup_Trattata As Decimal = input.sup_Trattata

            Dim acquaTot As Decimal = Utility.GetAcquaTotale(input.risorsaAcqua, sup_Trattata)

            original_doseTot_converted = AgronicaCoreMetaSchemaDAL.UnitaMisura_R.Converti(original_udm_cod, original_doseTot_converted, new_udm_cod)

            original_doseTot_converted = Utility.ConvertValueFromTipoControllo(original_doseTot_converted, new_tipo_controllo_cod)

            If input.flagTipoDose = enum_TipoMezzo.Ettaro OrElse input.flagTipoDose = enum_TipoMezzo.Quintale Then

                original_doseHa_converted = AgronicaCoreMetaSchemaDAL.UnitaMisura_R.Converti(original_udm_cod, original_doseHa_converted, new_udm_cod)

                original_doseHa_converted = Utility.ConvertValueFromTipoControllo(original_doseHa_converted, new_tipo_controllo_cod)

                If input.flagDoseQuantitaTotale = enum_doseQuantitaTotale.Qta_Totale Then

                    If sup_Trattata > 0 Then

                        original_doseHa_converted = original_doseTot_converted / sup_Trattata

                        original_doseHa_converted = Utility.ConvertValueFromTipoControllo(original_doseHa_converted, new_tipo_controllo_cod)

                        If acquaTot > 0 Then
                            original_doseHl_converted = original_doseTot_converted / acquaTot

                            original_doseHl_converted = Utility.ConvertValueFromTipoControllo(original_doseHl_converted, new_tipo_controllo_cod)
                        ElseIf acquaTot = 0 Then
                            original_doseHl_converted = 0
                        End If
                    End If

                Else

                    If sup_Trattata > 0 Then

                        original_doseTot_converted = sup_Trattata * original_doseHa_converted

                        original_doseTot_converted = Utility.ConvertValueFromTipoControllo(original_doseTot_converted, new_tipo_controllo_cod)

                        If acquaTot > 0 Then
                            original_doseHl_converted = original_doseTot_converted / acquaTot

                            original_doseHl_converted = Utility.ConvertValueFromTipoControllo(original_doseHl_converted, new_tipo_controllo_cod)

                        ElseIf acquaTot = 0 Then
                            original_doseHl_converted = 0
                        End If
                    End If

                End If

            Else

                original_doseHl_converted = AgronicaCoreMetaSchemaDAL.UnitaMisura_R.Converti(original_udm_cod, original_doseHl_converted, new_udm_cod)

                original_doseHl_converted = Utility.ConvertValueFromTipoControllo(original_doseHl_converted, new_tipo_controllo_cod)

                If input.flagDoseQuantitaTotale = enum_doseQuantitaTotale.Qta_Totale Then

                    If acquaTot > 0 Then
                        original_doseHl_converted = original_doseTot_converted / acquaTot

                        original_doseHl_converted = Utility.ConvertValueFromTipoControllo(original_doseHl_converted, new_tipo_controllo_cod)

                        If sup_Trattata > 0 Then
                            original_doseHa_converted = original_doseTot_converted / sup_Trattata

                            original_doseHa_converted = Utility.ConvertValueFromTipoControllo(original_doseHa_converted, new_tipo_controllo_cod)

                        End If
                    ElseIf acquaTot = 0 Then
                        original_doseHl_converted = 0
                    End If

                Else

                    If acquaTot > 0 Then

                        original_doseTot_converted = acquaTot * original_doseHl_converted

                        If sup_Trattata > 0 Then

                            original_doseTot_converted = Utility.ConvertValueFromTipoControllo(original_doseTot_converted, new_tipo_controllo_cod)

                            original_doseHa_converted = original_doseTot_converted / sup_Trattata

                            original_doseHa_converted = Utility.ConvertValueFromTipoControllo(original_doseHa_converted, new_tipo_controllo_cod)

                        End If

                    ElseIf acquaTot = 0 Then
                        original_doseHa_converted = 0
                    End If

                End If

            End If

        End If

        If original_doseHa_converted <> new_doseHa Then
            errori(TipoErrore.NonBloccante).Add(String.Format(My.Resources.AgronicaCoreMapper.DoseHaDiversaDa, input.Original_Row_Value.Dose_Ha, original_udm_sim))
        End If

        If original_doseHl_converted <> new_doseHl Then
            errori(TipoErrore.NonBloccante).Add(String.Format(My.Resources.AgronicaCoreMapper.DoseHlDiversaDa, input.Original_Row_Value.Dose_Hl, original_udm_sim))
        End If

        If original_doseTot_converted <> new_doseTot Then
            errori(TipoErrore.NonBloccante).Add(String.Format(My.Resources.AgronicaCoreMapper.DoseTotDiversaDa, input.Original_Row_Value.DoseTot_Ha, original_udm_sim))
        End If

    End Sub

    Private Sub CheckGiacenzaMagazzinoInternal(ByVal Piva_agenda As String,
                                               ByVal id_agenda_da_non_considerare As Integer,
                                               ByVal Piva_fabbricato As String,
                                               ByVal Sa_Cod_fabbricato As String,
                                               ByVal Fabbricato_Cod As String,
                                               ByVal descrizioneMagazzino As String,
                                               ByVal udmBase As UnitaDiMisura,
                                               ByVal elemCod As Integer,
                                               ByVal codiceProdotto As Integer,
                                               ByVal descrizioneProdotto As String,
                                               ByVal QtaTot As Decimal,
                                               ByVal Lotto As String,
                                               ByVal Data As Date,
                                               ByVal InfoOperazione As InfoOperazione,
                                               ByVal objParametri_Server As AgronicaCoreParametri,
                                               ByRef DictErroriTempXElemCod As Dictionary(Of Integer, List(Of ErroreGias)))

        Dim qta_in_data As Decimal = 0

        'guardo se la quantità è conforme per la data di intervento
        Dim objMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        qta_in_data = objMovDet.Verifica_Giacenze_Con_Magazzino_Esterno(Piva_fabbricato,
                                            Sa_Cod_fabbricato,
                                            Fabbricato_Cod,
                                            elemCod,
                                            If(InfoOperazione.IsSemina, 0, codiceProdotto),
                                            If(InfoOperazione.IsSemina, codiceProdotto, 0),
                                            0,
                                            0,
                                            Lotto,
                                            0,
                                            udmBase.codice,
                                            AGRODATAINIZIO,
                                            Data,
                                            Piva_agenda,
                                            0,
                                            id_agenda_da_non_considerare,
                                            objParametri_Server,
                                            conRaccoglitore:=True)

        If QtaTot > Math.Round(qta_in_data, 4) Then

            GestisciDictErroriXElemCod(elemCod,
                                      generaErroreGias(ErroreGias_Severity.Warning, "", String.Format(My.Resources.AgronicaCoreMapper.QuantitaNonSuffScarico, descrizioneProdotto, descrizioneMagazzino, CDate(Data).ToShortDateString, Math.Round(qta_in_data, 4), udmBase.descrizione), ""),
                                      DictErroriTempXElemCod)
        End If

        'guardo se la quantità di giacenza è conforme a prescindere dalla data
        qta_in_data = objMovDet.Verifica_Giacenze_Con_Magazzino_Esterno(Piva_fabbricato,
                                            Sa_Cod_fabbricato,
                                            Fabbricato_Cod,
                                            elemCod,
                                            If(InfoOperazione.IsSemina, 0, codiceProdotto),
                                            If(InfoOperazione.IsSemina, codiceProdotto, 0),
                                            0,
                                            0,
                                            Lotto,
                                            0,
                                            udmBase.codice,
                                            AGRODATAINIZIO,
                                            AGRODATAFINE,
                                            Piva_agenda,
                                            0,
                                            id_agenda_da_non_considerare,
                                            objParametri_Server,
                                            conRaccoglitore:=True)

        If QtaTot > Math.Round(qta_in_data, 4) Then

            GestisciDictErroriXElemCod(elemCod,
                                     generaErroreGias(ErroreGias_Severity.Warning, "", String.Format(My.Resources.AgronicaCoreMapper.GiacenzaNonSuffScarico, descrizioneProdotto, descrizioneMagazzino, Math.Round(qta_in_data, 4), udmBase.descrizione), ""),
                                      DictErroriTempXElemCod)
        End If

    End Sub


    Private Sub GestisciDictErroriXElemCod(ByVal elem_cod As Integer,
                                           ByVal errore As ErroreGias,
                                           ByRef DictErroriXElemCod As Dictionary(Of Integer, List(Of ErroreGias)))

        If Not IsNothing(DictErroriXElemCod) Then

            If DictErroriXElemCod.ContainsKey(elem_cod) Then
                DictErroriXElemCod.Item(elem_cod).Add(errore)
            Else
                DictErroriXElemCod.Add(elem_cod, New List(Of ErroreGias) From {errore})
            End If

        End If

    End Sub

End Class
