
Imports System.Data
Imports System.Data.Common
Imports System.Text.RegularExpressions
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Utility_KeyTranslator_W
    Inherits AgronicaCoreDataProvider.DataProvider




    '=========================================================================================================
    'Public Function KeyTranslator_CodContatto(
    '                                    ByVal Piva As String,
    '                                    ByVal CodContatto_OLD As String,
    '                                    ByVal CodContatto_NEW As String,
    '                                    ByRef objConnessione As DbConnection,
    '                                    ByRef objTransazione As DbTransaction,
    '                                    ByVal StringaConnessione As String,
    '                                    ByVal DirectoryLOG As String,
    '                                    ByVal FileLOG As String,
    '                                    ByVal IdentificatoreUtente As String) _
    '                                        As Boolean

    '    '----------------------------------------------------------------------

    '    Dim NomeRoutine As String = "AgronicaCoreVarieBIZ.Utility_KeyTranslator.KeyTranslator_CodContatto()"

    '    '----------------------------------------------------------------------

    '    Const c_NomeColonnaCercata = "cod_contatto"

    '    Dim ObjAgroDatabaseUtility As AgronicaCoreVarieDAL.System_Database_R
    '    Dim ObjUpdate As AgronicaCoreVarieDAL.Utility_KeyTranslator_W

    '    Dim DtTabelle As DataTable
    '    Dim DtColonne As DataTable

    '    Dim i As Integer = 0
    '    Dim j As Integer = 0

    '    Dim NomeTabella As String
    '    Dim Risultato As Boolean

    '    '------------------------------
    '    Dim FlagTransazioneLocale As Boolean = False
    '    Dim FlagConnessioneLocale As Boolean = False

    '    Dim MessaggioErrore As String = ""
    '    Dim xRisp As Boolean = True
    '    '------------------------------

    '    Dim objConnessioneR As DbConnection

    '    Try

    '        '------------------------------

    '        'Se la connessione è chiusa la apro
    '        If objConnessione Is Nothing Then
    '            'Richiedo una connessione
    '            objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(DataProviderFactory.Instance.AggiustaStringaDiConnessione(StringaConnessione))
    '            objConnessione.Open()
    '            FlagConnessioneLocale = True
    '        End If

    '        If objTransazione Is Nothing Then
    '            'Inizializzo la transazione
    '            objTransazione = objConnessione.BeginTransaction
    '            FlagTransazioneLocale = True
    '        End If

    '        '------------------------------
    '        '------------------------------

    '        'Richiedo una connessione x la lettura
    '        objConnessioneR = DataProviderFactory.Instance.CreaNuovaConnessione(DataProviderFactory.Instance.AggiustaStringaDiConnessione(StringaConnessione))
    '        objConnessioneR.Open()


    '        '------------------------------

    '        ObjAgroDatabaseUtility = New AgronicaCoreVarieDAL.System_Database_R

    '        DtTabelle = ObjAgroDatabaseUtility.Leggi_ElencoNomiTabelle(
    '                                            objConnessioneR,
    '                                            StringaConnessione,
    '                                            DirectoryLOG,
    '                                            FileLOG,
    '                                            IdentificatoreUtente)


    '        'Se la query ha recuperato qualcosa ...
    '        If DtTabelle.Rows.Count > 0 Then

    '            'Per ogni tabella  ricavo i nomi di tutti i suoi campi
    '            For i = 0 To DtTabelle.Rows.Count - 1


    '                NomeTabella = DtTabelle.Rows(i).Item("TABLE_NAME")

    '                DtColonne = ObjAgroDatabaseUtility.Leggi_ElencoNomiColonneTabella(
    '                                            NomeTabella,
    '                                            objConnessioneR,
    '                                            StringaConnessione,
    '                                            DirectoryLOG,
    '                                            FileLOG,
    '                                            IdentificatoreUtente)

    '                'Se la query ha recuperato qualcosa ...
    '                If DtColonne.Rows.Count > 0 Then

    '                    For j = 0 To DtColonne.Rows.Count - 1

    '                        'Verifico se nella tabella esiste un campo con il nome giusto
    '                        If LCase(DtColonne.Rows(j).Item("COLUMN_NAME")) = c_NomeColonnaCercata Then

    '                            'Sostituisco la CodContatto_OLD con la CodContatto_NEW
    '                            ObjUpdate = New AgronicaCoreVarieDAL.Utility_KeyTranslator_W

    '                            Risultato = ObjUpdate.Modifica_CodContatto(
    '                                            NomeTabella,
    '                                            Piva,
    '                                            CodContatto_OLD,
    '                                            CodContatto_NEW,
    '                                            objConnessione,
    '                                            objTransazione,
    '                                            StringaConnessione,
    '                                            DirectoryLOG,
    '                                            FileLOG,
    '                                            IdentificatoreUtente)

    '                        End If

    '                    Next

    '                    DtColonne.Dispose()

    '                End If

    '            Next

    '            DtTabelle.Dispose()

    '        End If

    '        '------------------------------
    '        '------------------------------
    '        '------------------------------

    '        'Elimino tutti gli oggetti utilizzati

    '        ObjUpdate = Nothing
    '        ObjAgroDatabaseUtility = Nothing

    '        '------------------------------

    '        'Restituisco un valore Dummy
    '        xRisp = True

    '        'Se ho la transazione è stata avviata in questa routine faccio il commit
    '        If FlagTransazioneLocale = True Then
    '            objTransazione.Commit()
    '        End If

    '        '----------------------------------------------------------------------------

    '    Catch ex As Exception

    '        'Restituisco un valore Dummy
    '        xRisp = False

    '        'Faccio il rollback della transazione
    '        If Not objTransazione Is Nothing Then
    '            objTransazione.Rollback()
    '        End If

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    Finally

    '        'Chiudo la connessione se è stata aperta in questa routine
    '        If (FlagConnessioneLocale = True) AndAlso (Not objConnessione Is Nothing) Then
    '            objConnessione.Close()
    '            objConnessione.Dispose()
    '        End If

    '        objConnessioneR.Close()
    '        objConnessioneR.Dispose()

    '    End Try

    '    Return xRisp

    'End Function


    '=========================================================================================================
    Public Function KeyTranslator_CodContatto2(
                                        ByVal Piva As String,
                                        ByVal CodContatto_OLD As String,
                                        ByVal CodContatto_NEW As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                            As Boolean

        '----------------------------------------------------------------------

        Dim NomeRoutine As String = "AgronicaCoreVarieBIZ.Utility_KeyTranslator.KeyTranslator_CodContatto2()"

        '----------------------------------------------------------------------

        Const c_NomeColonnaCercata = "cod_contatto"

        Dim ObjAgroDatabaseUtility As AgronicaCoreVarieDAL.System_Database_R
        Dim ObjUpdate As AgronicaCoreVarieDAL.Utility_KeyTranslator_W

        Dim DtTabelle As DataTable
        Dim DtColonne As DataTable

        Dim i As Integer = 0
        Dim j As Integer = 0

        Dim NomeTabella As String
        Dim Risultato As Boolean

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------

        Dim xFiltro_Aggiuntivo_table_owner As String = "@table_owner = 'dbo'"

        Try

            '------------------------------

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)


            '------------------------------

            ObjAgroDatabaseUtility = New AgronicaCoreVarieDAL.System_Database_R

            DtTabelle = ObjAgroDatabaseUtility.Leggi_ElencoNomiTabelle_2(objParametri, xFiltro_Aggiuntivo_table_owner)

            If DtTabelle.Rows.Count > 0 Then

                'Per ogni tabella  ricavo i nomi di tutti i suoi campi
                For i = 0 To DtTabelle.Rows.Count - 1

                    NomeTabella = DtTabelle.Rows(i).Item("TABLE_NAME")

                    DtColonne = ObjAgroDatabaseUtility.Leggi_ElencoNomiColonneTabella_3(
                                                                            NomeTabella,
                                                                            objParametri)

                    If DtColonne.Rows.Count > 0 Then

                        ObjUpdate = New AgronicaCoreVarieDAL.Utility_KeyTranslator_W

                        For j = 0 To DtColonne.Rows.Count - 1

                            Select Case CStr(LCase(DtColonne.Rows(j).Item("name"))).ToLower

                                'cod_contatto
                                Case c_NomeColonnaCercata

                                    Select Case NomeTabella.ToLower

                                        '********************* NOTA X TABELLE SIAN ******************************
                                        '-	Tabella ws_RegVino_Soggetti: colonne CodiceSoggetto e CUAA 
                                        '-	Tabella ws_RegVino_LogInvio_DettaglioSoggetti: colonna ws_RegVino_Soggetto_Cod (in collegamento alla tabella sopra)
                                        '- ws_RegVino_Soggetti_xContatti non la vedo sul db… tabella vecchia? (era gestita qui nel case)
                                        'Non bisogna modificare questi campi perché il cod_contatto è anche una chiave per l’anagrafica su SIAN dei contatti, 
                                        'quindi se cambia il cod_contatto bisogna inviare una nuova anagrafica di contatto ma le operazioni già inviate non devono essere toccate
                                        '***************************************************************************************

                                        '********************** CASI PARTICOLARI DA GESTIRE *******************
                                        'Vans suggerisce di lasciar perdere la prenotazione piante:
                                        '-	Tabella Programmazione_entita colonna Veg_Cod_Cliente (Veg_Cod_Cliente = Vivaio Associato, sito prenotazione piante) @Vanni, confermi che in quella colonna c’è sempre e solo il cod_contatto?
                                        '-	Tabella Programmazione_Testata colonna piva: @Vanni questo è sempre e solo un cod_contatto? (sito prenotazione piante)

                                        Case "Liquidita".ToLower, "Agronica_Log_Contatti".ToLower
                                            'Queste tabelle non hanno data_creazione e data_modifica, passo quindi false
                                            Risultato = ObjUpdate.Modifica_CodContatto_TabellaGenerica(
                                                                NomeTabella,
                                                                c_NomeColonnaCercata,
                                                                Piva,
                                                                CodContatto_OLD,
                                                                CodContatto_NEW,
                                                                False,
                                                                objParametri)

                                        Case Else

                                            Risultato = ObjUpdate.Modifica_CodContatto_TabellaGenerica(
                                                                NomeTabella,
                                                                c_NomeColonnaCercata,
                                                                Piva,
                                                                CodContatto_OLD,
                                                                CodContatto_NEW,
                                                                True,
                                                                objParametri)
                                    End Select

                                Case "riferimento".ToLower

                                    If NomeTabella.ToLower = "Liquidita".ToLower Then

                                        'la tabella Liquidita ha il cod_contatto anche nel campo Riferimento
                                        Risultato = ObjUpdate.Modifica_Riferimento_Liquidita(Piva,
                                                                    CodContatto_OLD,
                                                                    CodContatto_NEW,
                                                                    objParametri)

                                    End If

                                Case "cod_contatto_terzi"

                                    If NomeTabella.ToLower = "Linee_Produzioni".ToLower Then

                                        Risultato = ObjUpdate.Modifica_CodContatto_TabellaGenerica(
                                                                NomeTabella,
                                                                "cod_contatto_terzi",
                                                                Piva,
                                                                CodContatto_OLD,
                                                                CodContatto_NEW,
                                                                True,
                                                                objParametri)

                                    End If


                                Case "utente"

                                    'lab CQ fruttagel, caso particolare
                                    If NomeTabella.ToLower = "LCQ_ParametriValori".ToLower Then
                                        'In queste tabelle, essendoci il cod_contatto invece del cod_risum, ed essendo la chiave del contatto piva & cod_contatto,
                                        'l'update va fatto con:
                                        'LCQ_ParametriValori: Utente è il cod_contatto, devo usare Piva_SuperUser per la piva?
                                        'RISPOSTA: Sì, i contatti sono sull’impresa superuser, quindi puoi usare quella piva lì…

                                        Risultato = ObjUpdate.Modifica_CodContattoxLABCQ(CodContatto_OLD,
                                                                                        CodContatto_NEW,
                                                                                        objParametri)


                                    End If

                                Case "codice_fiscale"

                                    If NomeTabella.ToLower = "Contatti".ToLower Then

                                        Risultato = ObjUpdate.Modifica_CodContatto_TabellaGenerica(
                                                                                                    NomeTabella,
                                                                                                    "codice_fiscale",
                                                                                                    Piva,
                                                                                                    CodContatto_OLD,
                                                                                                    CodContatto_NEW,
                                                                                                    True,
                                                                                                    objParametri)

                                    End If

                            End Select 'nome colonna

                        Next

                        DtColonne.Dispose()

                    End If

                Next 'elenco tabelle

                DtTabelle.Dispose()

                '********************** CASI PARTICOLARI DA GESTIRE *******************
                '-	Tabella Reg_Impianti_Codici colonna val_Cod quando id_cod=1074 (organismo referente)
                'c'è solo cod_contatto, non si filtra la piva

                Risultato = ObjUpdate.Modifica_CodContatto_OrganismoReferente(CodContatto_OLD,
                                                                            CodContatto_NEW,
                                                                                objParametri)

                '-----------------------------------------------------------------
                'GESTIONE UPDATE VAL_COD SU TABELLA Imprese_Codici se ID_COD = enum_CodiciAnagrafe.Organismo_Referente o enum_CodiciAnagrafe.Impianto_Cooperativa (1074)

                Dim ObjUpdateImprese_Codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Write

                Dim xFiltroAggiuntivo_UpdateImprese_Codici As String = "Val_Cod = '" & CodContatto_OLD & "'"

                ObjUpdateImprese_Codici.Modifica(Piva,
                                                 enum_CodiciAnagrafe.Organismo_Referente,
                                                 CodContatto_NEW,
                                                 Nothing,
                                                 Nothing,
                                                 xFiltroAggiuntivo_UpdateImprese_Codici,
                                                 objParametri)
                '-----------------------------------------------------------------

            End If

            '------------------------------
            '------------------------------
            '------------------------------
            'Elimino tutti gli oggetti utilizzati
            ObjUpdate = Nothing
            ObjAgroDatabaseUtility = Nothing

            '------------------------------

            'Restituisco un valore Dummy
            xRisp = True

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            '----------------------------------------------------------------------------

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objConnessione Is Nothing AndAlso
               Not objParametri.objTransazione Is Nothing AndAlso
               FlagTransazioneLocale = True Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            MessaggioErrore = String.Format("[Tabella = {0}] " & ex.Message & " [{1}]",
                                            NomeTabella, If(ex.InnerException IsNot Nothing, ex.InnerException.Message, ""))
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return xRisp

    End Function

    '=========================================================================================================
    Public Function KeyTranslator_PartitaIVA(
                                        ByVal Piva_OLD As String,
                                        ByVal Piva_NEW As String,
                                        ByVal Flag_CambiaSuperUser As Boolean,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        '----------------------------------------------------------------------

        Dim NomeRoutine As String = "AgronicaCoreVarieBIZ.Utility_KeyTranslator.KeyTranslator_PartitaIVA()"

        '----------------------------------------------------------------------

        'Const c_NomeColonnaCercata = "piva"

        Dim ObjAgroDatabaseUtility As AgronicaCoreVarieDAL.System_Database_R
        Dim ObjUpdate As New AgronicaCoreVarieDAL.Utility_KeyTranslator_W
        Dim ObjContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

        Dim DtTabelle_Server As DataTable
        Dim DtColonne_Server As DataTable
        Dim DtContatti_Server As DataTable

        Dim DtTabelle_Utenti As DataTable
        Dim DtColonne_Utenti As DataTable

        Dim i As Integer = 0
        Dim j As Integer = 0

        Dim NomeTabella As String

        Dim xFiltroAggiuntivo As String = ""

        '------------------------------
        Dim FlagConnessioneLocale_Server As Boolean = False
        Dim FlagTransazioneLocale_Server As Boolean = False

        Dim FlagConnessioneLocale_Utenti As Boolean = False
        Dim FlagTransazioneLocale_Utenti As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True
        Dim Flag_Risp As Boolean = True
        '------------------------------

        Dim xFiltro_Aggiuntivo_table_owner As String = "@table_owner = 'dbo'"

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale_Server,
                                                                                    FlagTransazioneLocale_Server,
                                                                                    objParametri_Server)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale_Utenti,
                                                                                    FlagTransazioneLocale_Utenti,
                                                                                    objParametri_Utenti)

            '=========================================================
            'NOTA X IL DEBUG:
            'dato che la query usa objParametri.PivaSuperUser
            'il gias_super_server deve avere la pivasuperuser giusta, altrimenti la query non trova risultato
            '(può succedere se si fanno tentativi di cambio piva superuser consecutivi senza aggiornamento super_server)

            'Modifico la piva nel campo cod_contatto
            DtContatti_Server = ObjContatti.Contatti_Contatto_Leggi("", Piva_OLD, 0, 0, False, False, 0,
                                                             0, False, 0, -99, 0, "", True, 0, 0, 0,
                                                             0, 0,
                                                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                             "", "", objParametri_Server)

            If DtContatti_Server.Rows.Count > 0 Then

                For i = 0 To DtContatti_Server.Rows.Count - 1

                    KeyTranslator_CodContatto2(CStr(DtContatti_Server.Rows(i).Item("Piva")),
                                                Piva_OLD,
                                                Piva_NEW,
                                                objParametri_Server)
                Next

            End If
            '=========================================================



            ObjAgroDatabaseUtility = New AgronicaCoreVarieDAL.System_Database_R

            DtTabelle_Server = ObjAgroDatabaseUtility.Leggi_ElencoNomiTabelle_2(objParametri_Server, xFiltro_Aggiuntivo_table_owner)


            'Se la query ha recuperato qualcosa ...
            If DtTabelle_Server.Rows.Count > 0 Then

                'Per ogni tabella  ricavo i nomi di tutti i suoi campi
                For i = 0 To DtTabelle_Server.Rows.Count - 1

                    NomeTabella = DtTabelle_Server.Rows(i).Item("TABLE_NAME")


                    DtColonne_Server = ObjAgroDatabaseUtility.Leggi_ElencoNomiColonneTabella_3(
                                                        NomeTabella,
                                                        objParametri_Server)


                    'Se la query ha recuperato qualcosa ...
                    If DtColonne_Server.Rows.Count > 0 Then

                        'Disabilito il CaseSensitive nel datatable per il confronto tra stringhe con il select
                        DtColonne_Server.CaseSensitive = False

                        For j = 0 To DtColonne_Server.Rows.Count - 1

                            If (LCase(DtColonne_Server.Rows(j).Item("name")) = "piva") Or
                                (LCase(DtColonne_Server.Rows(j).Item("name")) = "figlio") Or
                                (LCase(DtColonne_Server.Rows(j).Item("name")) = "piva_rif") Or
                                (LCase(DtColonne_Server.Rows(j).Item("name")) = "notifica_piva") Or
                                (LCase(DtColonne_Server.Rows(j).Item("name")) = "unitaproduttiva_piva") Or
                                (LCase(DtColonne_Server.Rows(j).Item("name")) = "centropreparazione_piva") Or
                                (LCase(DtColonne_Server.Rows(j).Item("name")) = "centroricevimento_piva") Or
                                (LCase(DtColonne_Server.Rows(j).Item("name")) = "pap_piva") Or
                                (LCase(DtColonne_Server.Rows(j).Item("name")) = "pc_dettagli_piva") Or
                                (LCase(DtColonne_Server.Rows(j).Item("name")) = "Allegati_Documenti_Piva".ToLower) Or
                                (LCase(DtColonne_Server.Rows(j).Item("name")) = "Analisi_Campione_Key_Piva".ToLower) Then

                                '

                                Select Case NomeTabella.ToLower

                                    '****************  SIAN *********************
                                    'per quanto riguarda il codOper delle tabelle del SIAN, si è deciso di non toccarlo: 
                                    'se cambia il codice fiscale dell'azienda è un fatto "importante"
                                    'da gestire all'occasione:
                                    '- se la piva cambia subito perchè è stata inserita su Gias per errore e magari ci sono già operazioni pronte per l'invio:
                                    '-> allora ci collegheremo e faremo query di update
                                    '- se cambia perchè è cambiata l'azienda -> ne parliamo
                                    'ws_RegVino_LogInvio_Dettaglio_Errori
                                    'ws_RegVino_LogInvio_DettaglioSoggetti
                                    'ws_RegVino_LogInvio_DettaglioVasi
                                    'ws_RegVino_Operazioni
                                    'ws_RegVino_Prodotti
                                    'ws_RegVino_Soggetti
                                    'ws_RegVino_Vasi
                                    'queste tabelle sono gestite per la colonna "piva:
                                    '- ws_Reg_Vino_Vasi_xCantina_Vasche
                                    '- ws_RegVino_Operazioni
                                    '- ws_RegVino_Operazioni_Prodotti
                                    '- ws_RegVino_Operazioni_xMovimentiDettagli
                                    '- ws_RegVino_Prodotti


                                    Case "GerarchiaImprese".ToLower

                                        Flag_Risp = ObjUpdate.Modifica_Piva_GerarchiaImprese(
                                                                Piva_OLD,
                                                                Piva_NEW,
                                                                objParametri_Server)

                                    Case "Mov_Dettagli_Riferimenti".ToLower

                                        Flag_Risp = ObjUpdate.Modifica_Piva_Mov_Dettagli_Riferimenti(
                                                                Piva_OLD,
                                                                Piva_NEW,
                                                                objParametri_Server)

                                    'Case "liquidita", "SDI_Log_Dettaglio".ToLower, "MVV_Log_Dettaglio".ToLower, "ws_Reg_Vino_Vasi_xCantina_Vasche".ToLower,
                                    '        "ws_RegVino_Operazioni".ToLower, "ws_RegVino_Operazioni_Prodotti".ToLower,
                                    '        "ws_RegVino_Operazioni_xMovimentiDettagli".ToLower, "ws_RegVino_Prodotti".ToLower,
                                    '        "__Tmp_Movimenti_dettagli".ToLower, "__Tmp_Movimenti_Destinazioni_DateDistinta".ToLower,
                                    '        "__Tmp_Movimenti_Destinazioni".ToLower, "__Tmp_Movimenti".ToLower, "__Tmp_Mov_dettaglioTecnico".ToLower,
                                    '        "__Tmp_Agenda".ToLower, "ACCDAA_ACC_RiepilogoInvioDati".ToLower,
                                    '        "Agronica_Log_Agenda".ToLower, "Alert_Area".ToLower, "PUA_Testata_Appoggio".ToLower,
                                    '     "PDC_Fast_Sblocca".ToLower, "PDC_Sblocca".ToLower, "Utenti_Visibilita_Appoggio".ToLower,
                                    '     "__Agenda_Delete_Agende".ToLower, "__Agenda_Rpt_Conc".ToLower

                                    '    'tabelle SENZA Username_Modifica e data_modifica

                                    '    Flag_Risp = ObjUpdate.Modifica_Piva_TabellaGenerica(
                                    '                            NomeTabella,
                                    '                            LCase(DtColonne_Server.Rows(j).Item("name")),
                                    '                            Piva_OLD,
                                    '                            Piva_NEW,
                                    '                            False,
                                    '                            objParametri_Server)

                                    'Case "SDI_Log".ToLower, "MVV_Log".ToLower
                                    '    'questa tabella ha data_modifica ma non username_modifica

                                    '    Flag_Risp = ObjUpdate.Modifica_Piva_TabellaGenerica_SoloDataModifica(
                                    '                            NomeTabella,
                                    '                            LCase(DtColonne_Server.Rows(j).Item("name")),
                                    '                            Piva_OLD,
                                    '                            Piva_NEW,
                                    '                            objParametri_Server)

                                    'Case "XML_Log".ToLower, "Ist_Credito".ToLower
                                    '    'questa tabella ha Username_Modifica ma non data_modifica

                                    '    Flag_Risp = ObjUpdate.Modifica_Piva_TabellaGenerica_SoloUsernameModifica(
                                    '                                                                  NomeTabella,
                                    '                                                                  LCase(DtColonne_Server.Rows(j).Item("name")),
                                    '                                                                  Piva_OLD,
                                    '                                                                  Piva_NEW,
                                    '                                                                  objParametri_Server)

                                    Case "Parco_Macchine_2".ToLower, "Import_Analisi".ToLower, "Import_Atomizzatori".ToLower, "Import_Impolveratrici".ToLower
                                        'queste tabelle ci sono solo da Terremerse
                                        '-> non faccio niente

                                    Case Else
                                        Gestione_Update_Username_Modifica_Data_Modifica(DtColonne_Server, NomeTabella,
                                                                                    LCase(DtColonne_Server.Rows(j).Item("name")),
                                                                                    Piva_OLD, Piva_NEW, objParametri_Server)

                                End Select

                            ElseIf LCase(DtColonne_Server.Rows(j).Item("name")) = "Tipo".ToLower Then

                                'Aggiorno anche le tabelle Agronica_Log_* perché potrebbero avere la Piva salvate in delle colonne che non si chiamano Piva

                                xFiltroAggiuntivo = ""

                                Select Case NomeTabella.ToLower

                                    Case "Agronica_Log_Anagrafe".ToLower

                                        xFiltroAggiuntivo += "Tipo IN ('Reg_Impianti','Imprese_Progetti')"

                                        Flag_Risp = ObjUpdate.Modifica_Piva_TabellaGenerica(
                                                                                            NomeTabella,
                                                                                            "Param1",
                                                                                            Piva_OLD,
                                                                                            Piva_NEW,
                                                                                            False,
                                                                                            objParametri_Server,
                                                                                            xFiltroAggiuntivo)

                                        xFiltroAggiuntivo = ""

                                    Case "Agronica_Log_Ricette".ToLower

                                        xFiltroAggiuntivo += "Tipo = 'Ricette'"

                                        Flag_Risp = ObjUpdate.Modifica_Piva_TabellaGenerica(
                                                                                            NomeTabella,
                                                                                            "Param2",
                                                                                            Piva_OLD,
                                                                                            Piva_NEW,
                                                                                            False,
                                                                                            objParametri_Server,
                                                                                            xFiltroAggiuntivo)

                                        xFiltroAggiuntivo = ""

                                        xFiltroAggiuntivo += "Tipo = 'Ricette_Operazioni'"

                                        Flag_Risp = ObjUpdate.Modifica_Piva_TabellaGenerica(
                                                                                            NomeTabella,
                                                                                            "Param3",
                                                                                            Piva_OLD,
                                                                                            Piva_NEW,
                                                                                            False,
                                                                                            objParametri_Server,
                                                                                            xFiltroAggiuntivo)

                                        xFiltroAggiuntivo = ""

                                    Case "Agronica_Log_Pua".ToLower

                                        xFiltroAggiuntivo += "Tipo = 'PUA_Testata'"

                                        Flag_Risp = ObjUpdate.Modifica_Piva_TabellaGenerica(
                                                                                            NomeTabella,
                                                                                            "Param3",
                                                                                            Piva_OLD,
                                                                                            Piva_NEW,
                                                                                            False,
                                                                                            objParametri_Server,
                                                                                            xFiltroAggiuntivo)

                                        xFiltroAggiuntivo = ""

                                        xFiltroAggiuntivo += "Tipo IN ('Anagrafe_VincoliAgronomici','PUA_LetamazioniPrecedenti')"

                                        Flag_Risp = ObjUpdate.Modifica_Piva_TabellaGenerica(
                                                                                            NomeTabella,
                                                                                            "Param4",
                                                                                            Piva_OLD,
                                                                                            Piva_NEW,
                                                                                            False,
                                                                                            objParametri_Server,
                                                                                            xFiltroAggiuntivo)
                                        xFiltroAggiuntivo = ""

                                End Select

                            End If  'colonna piva o figlio

                            '================================================================
                            '============= MODIFICA SUPERUSER =======================
                            'la modifica superuser si riesce a fare sui clienti GIASLAN
                            'sui clienti GIASONLINE si riesce solo se non hanno dati nelle tabelle dove la pivasuperuser è foreign key
                            '(ad esempio: alert_elenco, alert_entita, ecc)
                            '================================================================
                            If Flag_CambiaSuperUser Then

                                If (LCase(DtColonne_Server.Rows(j).Item("name")) = "superuser") Or
                                    (LCase(DtColonne_Server.Rows(j).Item("name")) = "user") Or
                                    (LCase(DtColonne_Server.Rows(j).Item("name")) = "piva_superuser") Or
                                    (LCase(DtColonne_Server.Rows(j).Item("name")) = "pivasuperuser") Or
                                    (LCase(DtColonne_Server.Rows(j).Item("name")) = "audit_superuser") Or
                                    (LCase(DtColonne_Server.Rows(j).Item("name")) = "intervista_superuser") Or
                                    (LCase(DtColonne_Server.Rows(j).Item("name")) = "notifica_superuser") Or
                                    (LCase(DtColonne_Server.Rows(j).Item("name")) = "username_creazione") Or
                                    (LCase(DtColonne_Server.Rows(j).Item("name")) = "Analisi_SuperUser".ToLower) Or
                                    (LCase(DtColonne_Server.Rows(j).Item("name")) = "PC_SuperUser".ToLower) Or
                                    (LCase(DtColonne_Server.Rows(j).Item("name")) = "Allegati_Documenti_Superuser".ToLower) Or
                                    (LCase(DtColonne_Server.Rows(j).Item("name")) = "Ricetta_SuperUser".ToLower) Then

                                    '15/10/2019: commentato perchè altrimenti ci sarebbe il doppio update nella staessa query
                                    '(LCase(DtColonne.Rows(j).Item("name")) = "username_modifica") Or _

                                    Select Case NomeTabella.ToLower

                                        '    Case "PUA_Consistenze_Animali_Appoggio".ToLower, "PUA_Effluente_Appoggio".ToLower,
                                        '    "PUA_Programmazione_Appoggio".ToLower, "PUA_Testata_Appoggio".ToLower, "PUA_TrattamentoEffluenti_Appoggio".ToLower,
                                        '    "Analisi_Appoggio_Import".ToLower, "Utenti_Visibilita_Appoggio".ToLower,
                                        '    "Agronica_Log_Agenda".ToLower, "Agronica_Log_Anagrafe".ToLower,
                                        '    "PUA_Testata_Appoggio".ToLower, "PDC_Sblocca".ToLower, "Analisi_Conformita_Capitolato_Cliente".ToLower,
                                        '    "Analisi_Conformita_DP".ToLower, "CAC_Codifica_UnitaMisura".ToLower, "Configurazione_Siti".ToLower,
                                        '    "liquidita", "PDC_Campioni".ToLower, "PDC_Codice_Analisi_Fruttagel_VEG_COD".ToLower, "PDC_LFO".ToLower,
                                        '    "Agronica_Log_Pua".ToLower, "Agronica_Log_Ricette".ToLower, "Alert_Area".ToLower

                                        '        'tabelle SENZA Username_Modifica e data_modifica
                                        '        ObjUpdate.Modifica_Piva_TabellaGenerica(NomeTabella,
                                        '                                                LCase(DtColonne_Server.Rows(j).Item("name")),
                                        '                                                Piva_OLD,
                                        '                                                Piva_NEW,
                                        '                                                False,
                                        '                                                objParametri_Server)

                                        '    Case "CAC_Codifica_Zone".ToLower, "CAC_Codifica_Portinnesti".ToLower, "CAC_Codifica_OTE".ToLower, "CAC_Codifica_Macchine".ToLower,
                                        '    "CAC_Codifica_ImpiantiIrrigazioni".ToLower, "CAC_Codifica_FormeAllevamento".ToLower, "CAC_Codifica_Fabbricati".ToLower,
                                        '    "CAC_Codifica_Animali".ToLower
                                        '        'tabelle con data_modifica ma non username_modifica

                                        '        Flag_Risp = ObjUpdate.Modifica_Piva_TabellaGenerica_SoloDataModifica(
                                        '                                NomeTabella,
                                        '                                LCase(DtColonne_Server.Rows(j).Item("name")),
                                        '                                Piva_OLD,
                                        '                                Piva_NEW,
                                        '                                objParametri_Server)

                                        '    Case "XML_Log".ToLower, "Ist_Credito".ToLower
                                        '        'questa tabella ha Username_Modifica ma non data_modifica

                                        '        Flag_Risp = ObjUpdate.Modifica_Piva_TabellaGenerica_SoloUsernameModifica(
                                        '                                                                      NomeTabella,
                                        '                                                                      LCase(DtColonne_Server.Rows(j).Item("name")),
                                        '                                                                      Piva_OLD,
                                        '                                                                      Piva_NEW,
                                        '                                                                      objParametri_Server)

                                        Case Else
                                            Gestione_Update_Username_Modifica_Data_Modifica(DtColonne_Server, NomeTabella,
                                                                                    LCase(DtColonne_Server.Rows(j).Item("name")),
                                                                                    Piva_OLD, Piva_NEW, objParametri_Server)

                                    End Select

                                End If

                            End If

                        Next j

                        DtColonne_Server.Dispose()

                    End If  '---------- DtColonne.Rows.Count > 0

                Next i

                DtTabelle_Server.Dispose()

            End If  '---------- DtTabelle.Rows.Count > 0 


            '"piva": "00000000001"

            '-----------------------------------------------------------------
            'GESTIONE UPDATE PIVA SU TABELLA KendoCache
            ObjUpdate.Replace_Piva_KendoCache(Piva_OLD,
                                            Piva_NEW,
                                            objParametri_Server)
            '-----------------------------------------------------------------

            '-----------------------------------------------------------------
            'GESTIONE UPDATE PIVA SU TABELLA AUDIT
            ObjUpdate.Replace_Piva_AuditRiferimentoCod(Piva_OLD,
                                                        Piva_NEW,
                                                        objParametri_Server)
            '-----------------------------------------------------------------

            '-----------------------------------------------------------------
            'GESTIONE UPDATE PIVA SU TABELLA Alert_EntitaxIndici
            ObjUpdate.Replace_Piva_AlertEntitaxIndici(Piva_OLD,
                                                        Piva_NEW,
                                                        objParametri_Server)
            '-----------------------------------------------------------------

            '-----------------------------------------------------------------
            'GESTIONE UPDATE VAL_COD SU TABELLA Imprese_Codici se ID_COD = enum_CodiciAnagrafe.CodiceCUAA (1010)

            Dim ObjUpdateImprese_Codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Write

            Dim xFiltroAggiuntivo_UpdateImprese_Codici As String = "Piva = '" & Piva_NEW & "' AND Val_Cod = '" & Piva_OLD & "'"

            ObjUpdateImprese_Codici.Modifica(Piva_NEW,
                                             enum_CodiciAnagrafe.CodiceCUAA,
                                             Piva_NEW,
                                             Nothing,
                                             Nothing,
                                             xFiltroAggiuntivo_UpdateImprese_Codici,
                                             objParametri_Server)
            '-----------------------------------------------------------------

            '#################################################
            '########## Update su DATABASE UTENTI ###############
            '#################################################


            'Aggiorno il campo piva superuser anche nelle tabelle del db Utenti 

            ObjAgroDatabaseUtility = New AgronicaCoreVarieDAL.System_Database_R

            DtTabelle_Utenti = ObjAgroDatabaseUtility.Leggi_ElencoNomiTabelle_2(objParametri_Utenti, xFiltro_Aggiuntivo_table_owner)

            'Se la query ha recuperato qualcosa ...
            If DtTabelle_Utenti.Rows.Count > 0 Then

                i = 0
                j = 0

                'Per ogni tabella  ricavo i nomi di tutti i suoi campi
                For i = 0 To DtTabelle_Utenti.Rows.Count - 1

                    NomeTabella = DtTabelle_Utenti.Rows(i).Item("TABLE_NAME")


                    DtColonne_Utenti = ObjAgroDatabaseUtility.Leggi_ElencoNomiColonneTabella_3(
                                                            NomeTabella,
                                                            objParametri_Utenti)


                    'Se la query ha recuperato qualcosa ...
                    If DtColonne_Utenti.Rows.Count > 0 Then

                        'Disabilito il CaseSensitive nel datatable per il confronto tra stringhe con il select
                        DtColonne_Utenti.CaseSensitive = False

                        For j = 0 To DtColonne_Utenti.Rows.Count - 1

                            If (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "piva") Or
                                (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "piva_azienda") Or
                                (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "piva_rif") Or
                                (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "notifica_piva") Or
                                (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "unitaproduttiva_piva") Or
                                (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "centropreparazione_piva") Or
                                (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "centroricevimento_piva") Or
                                (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "pap_piva") Or
                                (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "pc_dettagli_piva") Or
                                (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "Allegati_Documenti_Piva".ToLower) Or
                                (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "Analisi_Campione_Key_Piva".ToLower) Then

                                Gestione_Update_Username_Modifica_Data_Modifica(DtColonne_Utenti, NomeTabella,
                                                                                    LCase(DtColonne_Utenti.Rows(j).Item("name")),
                                                                                    Piva_OLD, Piva_NEW, objParametri_Utenti)


                            End If

                            If Flag_CambiaSuperUser Then
                                If (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "superuser") Or
                                    (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "user") Or
                                    (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "piva_superuser") Or
                                    (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "pivasuperuser") Or
                                    (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "audit_superuser") Or
                                    (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "intervista_superuser") Or
                                    (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "notifica_superuser") Or
                                    (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "username_creazione") Or
                                    (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "Analisi_SuperUser".ToLower) Or
                                    (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "PC_SuperUser".ToLower) Or
                                    (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "Allegati_Documenti_Superuser".ToLower) Or
                                    (LCase(DtColonne_Utenti.Rows(j).Item("name")) = "Ricetta_SuperUser".ToLower) Then



                                    Gestione_Update_Username_Modifica_Data_Modifica(DtColonne_Utenti, NomeTabella,
                                                                                        DtColonne_Utenti.Rows(j).Item("name"),
                                                                                        Piva_OLD, Piva_NEW, objParametri_Utenti)


                                End If
                            End If


                        Next j

                        DtColonne_Utenti.Dispose()

                    End If
                Next i

                DtTabelle_Utenti.Dispose()

            End If

            If Flag_CambiaSuperUser Then
                'si sta modificando la partita del superuser
                'occorre aggiornare la piva anche nella tabella Utenti_Dettagli del db Utenti

                Flag_Risp = ObjUpdate.Modifica_Piva_Utenti_Dettagli(Piva_NEW,
                                                                        False,
                                                                        objParametri_Utenti)
            End If


            '-----------------------------------------------------------------
            'GESTIONE UPDATE PIVA SU TABELLA Utenti_Impostazioni
            ObjUpdate.Modifica_Piva_TabellaGenerica("Utenti_Impostazioni",
                                                    "Piva_SuperUser",
                                                    Piva_OLD,
                                                    Piva_NEW,
                                                    True,
                                                    objParametri_Utenti)
            '-----------------------------------------------------------------



            '-----------------------------------------------------------------
            'GESTIONE UPDATE PIVA SU TABELLA Gruppi_Utente
            ObjUpdate.Modifica_Piva_TabellaGenerica("Gruppi_Utente",
                                                    "gruppi_utente_identificativo",
                                                    Piva_OLD,
                                                    Piva_NEW,
                                                    True,
                                                    objParametri_Utenti)
            '-----------------------------------------------------------------

            '-----------------------------------------------------------------
            'GESTIONE UPDATE PIVA SU TABELLA Utenti_Profili
            ObjUpdate.Replace_Piva_UtentiProfili("descrizione_1",
                                                    Piva_OLD,
                                                    Piva_NEW,
                                                    objParametri_Utenti)

            ObjUpdate.Replace_Piva_UtentiProfili("descrizione_2",
                                                    Piva_OLD,
                                                    Piva_NEW,
                                                    objParametri_Utenti)
            '-----------------------------------------------------------------

            'Update altre tabelle
            If Flag_CambiaSuperUser Then

                UtilityCambioCFUtente(Piva_OLD, Piva_NEW, 1, objParametri_Server, objParametri_Utenti)

            End If

            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            ObjUpdate = Nothing
            ObjAgroDatabaseUtility = Nothing
            ObjContatti = Nothing

            '------------------------------

            xRisp = True

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale_Server, objParametri_Server)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale_Utenti, objParametri_Utenti)

            '----------------------------------------------------------------------------

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri_Server.objConnessione Is Nothing AndAlso
               Not objParametri_Server.objTransazione Is Nothing AndAlso
               FlagTransazioneLocale_Server = True Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            'Faccio il rollback della transazione
            If Not objParametri_Utenti.objConnessione Is Nothing AndAlso
               Not objParametri_Utenti.objTransazione Is Nothing AndAlso
               FlagTransazioneLocale_Utenti = True Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Utenti)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = "[Tabella = " & NomeTabella & "](Piva_New=" & Piva_NEW & "Piva_Old=" & Piva_OLD & ")" &
                              " : " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)

            Scrivi_LOG(objParametri_Utenti, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale_Server, objParametri_Server)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale_Utenti, objParametri_Utenti)

        End Try

        Return xRisp

    End Function

    Public Function UtilityCambioCFUtente(ByVal Codice_Fiscale_Old As String, ByVal Codice_Fiscale_New As String, ByVal Flag_Azienda_Persona As Integer, ByVal objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim rval As New RispostaStandard
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreVarieBIZ.UtilityCambioCFUtente()"
        Dim MessaggioErrore As String
        Dim FlagTransazioneLocale_Server As Boolean = False
        Dim FlagConnessioneLocale_Server As Boolean = False
        Dim FlagTransazioneLocale_Utenti As Boolean = False
        Dim FlagConnessioneLocale_Utenti As Boolean = False

        Try
            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale_Server,
                                                                                    FlagTransazioneLocale_Server,
                                                                                    objParametri_Server)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale_Utenti,
                                                                                    FlagTransazioneLocale_Utenti,
                                                                                    objParametri_Utenti)

            'CONTROLLI PRELIMINARI
            Dim objUtenti_Dettagli_R As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
            Dim objUtenti_Dettagli_W As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W
            Dim objUtenti_Visibilita_Appoggio_R As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim objUtenti_Profili As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            Dim obj_Utenti_DBServer As New AgronicaCoreUtentiDAL.Utenti_Write
            Dim obj_QueryParametrizzata As New AgronicaCoreUtility.QueryParametrizzata_W
            Dim obj_System_Database As New AgronicaCoreVarieDAL.System_Database_R
            Dim Username As String = ""

            Dim NumeroRecordModificati = 0

            Dim ProcediConQuerySuUsernameCreazioneModifica As Boolean = True

            Dim DT_Utenti_Profili As New DataTable

            Dim DT_Utenti_Visibilita As New DataTable

            Dim n_record As Integer = 0

            Dim dt = objUtenti_Dettagli_R.Utenti_Dettagli_from_CF(Codice_Fiscale_Old, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Username = dt.Rows(0)("Username")
            objUtenti_Dettagli_W.Modifica_Parametrizzata(Username, "CodFisc", Codice_Fiscale_New, "", objParametri_Utenti, True)

            'Modifico anche la colonna CODICE_FISCALE nella tabella Utenti del DB server
            obj_Utenti_DBServer.Modifica_Codice_Fiscale(Username, Codice_Fiscale_New, Codice_Fiscale_Old, objParametri_Server)

            NumeroRecordModificati += 2

            n_record = 0

            obj_QueryParametrizzata.Modifica_Parametrizzata("UtentixCodFisc", "CodFisc", Codice_Fiscale_New, "Where CodFisc = '" & Agro_SQL_SaveText(Codice_Fiscale_Old) & "'", objParametri_Utenti, True, True, n_record)

            NumeroRecordModificati += n_record

            DT_Utenti_Profili = objUtenti_Profili.Leggi_2(Username, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                            "CAST(Utenti_Profili.Descrizione_2 AS nvarchar(max))  <> ''", "", objParametri_Utenti)


            If DT_Utenti_Profili.Rows.Count = 0 Then

                ProcediConQuerySuUsernameCreazioneModifica = True

            ElseIf DT_Utenti_Profili.Rows.Count > 0 Then

                DT_Utenti_Visibilita = objUtenti_Visibilita_Appoggio_R.Leggi(Username, 0, "", "", objParametri_Server)

                If DT_Utenti_Visibilita.Rows.Count = 0 Then

                    ProcediConQuerySuUsernameCreazioneModifica = False

                ElseIf DT_Utenti_Visibilita.Rows.Count > 0 Then

                    ProcediConQuerySuUsernameCreazioneModifica = True

                End If
            End If


            'Faccio l'Update di "Username_Creazione", "Username_Modifica" e delle altre tabelle in cui viene salvato il Codice_Fiscale
            'solo se ci sono dei record in Utenti_Visibilita_Appoggio e Utenti_Profili perchè ho la sicurezza che effetivamente
            'l'utente ha fatto il login su GIAS.
            If ProcediConQuerySuUsernameCreazioneModifica Then

                Dim List_ObjParametri As New List(Of AgronicaCoreParametri) From {objParametri_Server, objParametri_Utenti}
                Dim List_Colonne As New List(Of String) From {"Username_Creazione", "Username_Modifica"}

                obj_QueryParametrizzata.Modifica_Parametrizzata("Agenda", "Blocco_Username", Codice_Fiscale_New, "Where Blocco_Username = '" & Agro_SQL_SaveText(Codice_Fiscale_Old) & "'", objParametri_Server, True, True, n_record)

                NumeroRecordModificati += n_record

                n_record = 0

                obj_QueryParametrizzata.Modifica_Parametrizzata("Agronica_Log_Agenda", "Utente", Codice_Fiscale_New, "Where Utente = '" & Agro_SQL_SaveText(Codice_Fiscale_Old) & "'", objParametri_Server, False, True, n_record)

                NumeroRecordModificati += n_record

                n_record = 0

                obj_QueryParametrizzata.Modifica_Parametrizzata("Agronica_Log_Anagrafe", "Utente", Codice_Fiscale_New, "Where Utente = '" & Agro_SQL_SaveText(Codice_Fiscale_Old) & "'", objParametri_Server, False, True, n_record)

                NumeroRecordModificati += n_record

                n_record = 0

                obj_QueryParametrizzata.Modifica_Parametrizzata("Agronica_Log_Pua", "Utente", Codice_Fiscale_New, "Where Utente = '" & Agro_SQL_SaveText(Codice_Fiscale_Old) & "'", objParametri_Server, False, True, n_record)

                NumeroRecordModificati += n_record

                n_record = 0

                obj_QueryParametrizzata.Modifica_Parametrizzata("Agronica_Log_Ricette", "Utente", Codice_Fiscale_New, "Where Utente = '" & Agro_SQL_SaveText(Codice_Fiscale_Old) & "'", objParametri_Server, False, True, n_record)

                NumeroRecordModificati += n_record

                n_record = 0

                obj_QueryParametrizzata.Modifica_Parametrizzata("Appezzamento", "Blk_Fine_Username", Codice_Fiscale_New, "Where Blk_Fine_Username = '" & Agro_SQL_SaveText(Codice_Fiscale_Old) & "'", objParametri_Server, True, True, n_record)

                NumeroRecordModificati += n_record

                n_record = 0

                obj_QueryParametrizzata.Modifica_Parametrizzata("Appezzamento", "Blk_Inizio_Username", Codice_Fiscale_New, "Where Blk_Inizio_Username = '" & Agro_SQL_SaveText(Codice_Fiscale_Old) & "'", objParametri_Server, True, True, n_record)

                NumeroRecordModificati += n_record

                n_record = 0

                obj_QueryParametrizzata.Modifica_Parametrizzata("PianoConcimazione_Testata", "Blocco_Username", Codice_Fiscale_New, "Where Blocco_Username = '" & Agro_SQL_SaveText(Codice_Fiscale_Old) & "'", objParametri_Server, True, True, n_record)

                NumeroRecordModificati += n_record

                n_record = 0

                obj_QueryParametrizzata.Modifica_Parametrizzata("PUA_Testata", "Blocco_Username", Codice_Fiscale_New, "Where Blocco_Username = '" & Agro_SQL_SaveText(Codice_Fiscale_Old) & "'", objParametri_Server, True, True, n_record)

                NumeroRecordModificati += n_record

                n_record = 0

                obj_QueryParametrizzata.Modifica_Parametrizzata("Ricette", "Blocco_Username", Codice_Fiscale_New, "Where Blocco_Username = '" & Agro_SQL_SaveText(Codice_Fiscale_Old) & "'", objParametri_Server, True, True, n_record)

                NumeroRecordModificati += n_record

                For Each objParametri In List_ObjParametri

                    Dim dtTabelle = obj_System_Database.Leggi_ElencoNomiColonneTabelle_Data_Modifica(objParametri)

                    'Disabilito il CaseSensitive nel datatable per il confronto tra stringhe con il select
                    dtTabelle.CaseSensitive = False

                    For Each colonna In List_Colonne

                        Dim TrovataColonna = dtTabelle.Select("Nome_Colonna = '" & colonna & "'")

                        If Not IsNothing(TrovataColonna) AndAlso TrovataColonna.Length > 0 Then

                            Dim DT_Filtrato = TrovataColonna.CopyToDataTable

                            If Not IsNothing(DT_Filtrato) Then

                                For Each rowTabella In DT_Filtrato.Rows

                                    n_record = 0

                                    obj_QueryParametrizzata.Modifica_Parametrizzata(rowTabella("Nome_Tabella"), colonna, Codice_Fiscale_New, "Where " & colonna & " = '" & Agro_SQL_SaveText(Codice_Fiscale_Old) & "'", objParametri, CBool(rowTabella("Flag_DataModifica").ToString()), True, n_record)

                                    NumeroRecordModificati += n_record

                                Next

                            End If

                        End If


                    Next
                Next

            End If



            rval.RispostaOK = True

            If Flag_Azienda_Persona = 1 Then
                rval.RispostaStringa = "Partita IVA correttamente modificata, aggiornati " & NumeroRecordModificati & " record."
            ElseIf Flag_Azienda_Persona = 2 Then
                rval.RispostaStringa = "Codice fiscale correttamente modificato, aggiornati " & NumeroRecordModificati & " record."
            End If

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale_Server, objParametri_Server)

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale_Utenti, objParametri_Utenti)

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri_Server.objConnessione Is Nothing AndAlso
               Not objParametri_Server.objTransazione Is Nothing AndAlso
               FlagTransazioneLocale_Server = True Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            'Faccio il rollback della transazione
            If Not objParametri_Utenti.objConnessione Is Nothing AndAlso
               Not objParametri_Utenti.objTransazione Is Nothing AndAlso
               FlagTransazioneLocale_Utenti = True Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Utenti)
            End If

            MessaggioErrore = ex.Message

            rval.RispostaOK = False
            rval.Errore = MessaggioErrore

            Throw New Exception(MessaggioErrore)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale_Server, objParametri_Server)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale_Utenti, objParametri_Utenti)

        End Try
        Return rval
    End Function

    Private Sub Gestione_Update_Username_Modifica_Data_Modifica(ByVal DtColonne As DataTable,
                                                                ByVal NomeTabella As String,
                                                                ByVal NomeColonna As String,
                                                                ByVal Piva_OLD As String,
                                                                ByVal Piva_NEW As String,
                                                                ByRef objParametri As AgronicaCoreParametri)

        Dim ObjUpdate As New AgronicaCoreVarieDAL.Utility_KeyTranslator_W

        Dim Trovata_Colonna_Username_Modifica = DtColonne.Select("name = 'Username_Modifica'")

        Dim Trovata_Colonna_Data_Modifica = DtColonne.Select("name = 'Data_Modifica'")

        If Not IsNothing(Trovata_Colonna_Username_Modifica) AndAlso
           Not IsNothing(Trovata_Colonna_Data_Modifica) AndAlso
           Trovata_Colonna_Username_Modifica.Length = 0 AndAlso
           Trovata_Colonna_Data_Modifica.Length = 0 Then

            'tabelle SENZA Username_Modifica e data_modifica
            ObjUpdate.Modifica_Piva_TabellaGenerica(NomeTabella,
                                                            NomeColonna,
                                                            Piva_OLD,
                                                            Piva_NEW,
                                                            False,
                                                            objParametri)

        End If


        If Not IsNothing(Trovata_Colonna_Username_Modifica) AndAlso
           Not IsNothing(Trovata_Colonna_Data_Modifica) AndAlso
           Trovata_Colonna_Username_Modifica.Length = 0 AndAlso
           Trovata_Colonna_Data_Modifica.Length > 0 Then

            'tabelle con data_modifica ma non username_modifica

            ObjUpdate.Modifica_Piva_TabellaGenerica_SoloDataModifica(
                                                                    NomeTabella,
                                                                    NomeColonna,
                                                                    Piva_OLD,
                                                                    Piva_NEW,
                                                                    objParametri)

        End If


        If Not IsNothing(Trovata_Colonna_Username_Modifica) AndAlso
           Not IsNothing(Trovata_Colonna_Data_Modifica) AndAlso
           Trovata_Colonna_Username_Modifica.Length > 0 AndAlso
           Trovata_Colonna_Data_Modifica.Length = 0 Then

            'questa tabella ha Username_Modifica ma non data_modifica

            ObjUpdate.Modifica_Piva_TabellaGenerica_SoloUsernameModifica(
                                                                        NomeTabella,
                                                                        NomeColonna,
                                                                        Piva_OLD,
                                                                        Piva_NEW,
                                                                        objParametri)

        End If


        If Not IsNothing(Trovata_Colonna_Username_Modifica) AndAlso
           Not IsNothing(Trovata_Colonna_Data_Modifica) AndAlso
           Trovata_Colonna_Username_Modifica.Length > 0 AndAlso
           Trovata_Colonna_Data_Modifica.Length > 0 Then

            'tabelle con Username_Modifica e data_modifica
            ObjUpdate.Modifica_Piva_TabellaGenerica(
                                                     NomeTabella,
                                                     NomeColonna,
                                                     Piva_OLD,
                                                     Piva_NEW,
                                                     True,
                                                     objParametri)

        End If

    End Sub

End Class
