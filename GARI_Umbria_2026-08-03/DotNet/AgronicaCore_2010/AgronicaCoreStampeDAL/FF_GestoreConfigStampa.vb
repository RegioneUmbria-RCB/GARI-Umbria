

Imports System.Runtime.Remoting
Imports AgronicaCoreDataProvider

Public Class FF_GestoreConfigStampa

    Private Const LabelSeparator = "|"c
    Private Const LabelPrintSeparator = ","c
    Private Const SeparatoreCodiceQRY As String = "-"

    Private Const NomeTabella_AgendaxUpdate As String = "Movimenti_Dettagli"
    Private Const NomeColonna_AgendaxUpdate As String = "ID_Attivita"


    Public Function SalvaConfigurazioneDiStampa(ByVal configurazioneDaSalvare As String, ByVal objParametri_server As AgronicaCoreParametri) As String

        Dim curId_agenda As Integer
        Dim cur_id_mov_det As Integer
        Dim cur_old_id_mov_det As Integer
        Dim cur_numero_copie As Integer
        Dim cur_lingua_cod As Integer
        Dim cur_layout_cod As Integer
        Dim cur_stampante_cod As Integer
        Dim cur_Stampante_NomePerStampa As String = ""
        Dim cur_FF_Stampa_Dettagli_COD As Integer
        Dim cur_FF_Stampanti_cod_FF_Stampa_Dettagli_COD As String = ""

        Dim cur_OModuli_Referenze_Config_Testata As Integer

        Dim rVal As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String
        Dim NomeRoutine As String

        Dim scriviCfg As New FF_ConfigurazioniDiStampa_Dettagli_W

        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                FlagConnessioneLocale,
                FlagTransazioneLocale,
                objParametri_server)

            Dim lConfigurazioneDaSalvareSplit As String() = configurazioneDaSalvare.Split(LabelSeparator)
            getLayoutLinguaStampante(lConfigurazioneDaSalvareSplit(0), cur_OModuli_Referenze_Config_Testata, curId_agenda, cur_id_mov_det, cur_layout_cod, cur_stampante_cod, cur_Stampante_NomePerStampa, cur_lingua_cod, cur_numero_copie, cur_FF_Stampanti_cod_FF_Stampa_Dettagli_COD, cur_old_id_mov_det)

            GestioneConfig_PulisciDettagli(curId_agenda, NomeTabella_AgendaxUpdate, NomeColonna_AgendaxUpdate, objParametri_server)

            Dim listaMovDetStampanti As New List(Of FF_ConfigurazioneDiStampa_obj)


            For Each copia In lConfigurazioneDaSalvareSplit

                getLayoutLinguaStampante(copia, cur_OModuli_Referenze_Config_Testata, curId_agenda, cur_id_mov_det, cur_layout_cod, cur_stampante_cod, cur_Stampante_NomePerStampa, cur_lingua_cod, cur_numero_copie, cur_FF_Stampanti_cod_FF_Stampa_Dettagli_COD, cur_old_id_mov_det)

                Dim lCur_FF_Stampanti_cod_FF_Stampa_Dettagli_CODSplit As String() = cur_FF_Stampanti_cod_FF_Stampa_Dettagli_COD.Split(SeparatoreCodiceQRY)
                If lCur_FF_Stampanti_cod_FF_Stampa_Dettagli_CODSplit.Length = 1 Then
                    cur_FF_Stampa_Dettagli_COD = lCur_FF_Stampanti_cod_FF_Stampa_Dettagli_CODSplit(0)
                Else
                    cur_FF_Stampa_Dettagli_COD = lCur_FF_Stampanti_cod_FF_Stampa_Dettagli_CODSplit(1)
                End If


                Dim curLL_FF_Stampa_Dettagli_COD As Integer = 0

                If cur_old_id_mov_det <> 0 AndAlso cur_old_id_mov_det <> cur_id_mov_det Then

                    scriviCfg.Cancella(cur_FF_Stampa_Dettagli_COD, cur_stampante_cod, "", objParametri_server)
                    cur_FF_Stampa_Dettagli_COD = 0

                End If

                If cur_FF_Stampa_Dettagli_COD = 0 Then
                    curLL_FF_Stampa_Dettagli_COD = (
                        From ll In listaMovDetStampanti
                        Where ll.id_mov_det = cur_id_mov_det
                        Select ll.FF_Stampa_Dettagli_Cod
                        ).FirstOrDefault

                    If curLL_FF_Stampa_Dettagli_COD <> 0 Then

                        cur_FF_Stampa_Dettagli_COD = curLL_FF_Stampa_Dettagli_COD
                    End If
                End If

                GestioneConfig(
                    curId_agenda,
                    cur_id_mov_det,
                    cur_FF_Stampa_Dettagli_COD,
                    cur_layout_cod,
                    cur_stampante_cod,
                    cur_lingua_cod,
                    NomeTabella_AgendaxUpdate,
                    NomeColonna_AgendaxUpdate,
                    objParametri_server
                )

                If curLL_FF_Stampa_Dettagli_COD = 0 Then
                    listaMovDetStampanti.Add(New FF_ConfigurazioneDiStampa_obj With {.id_mov_det = cur_id_mov_det, .stampante_cod = cur_stampante_cod, .FF_Stampa_Dettagli_Cod = cur_FF_Stampa_Dettagli_COD})
                End If


                rVal &= cur_stampante_cod & SeparatoreCodiceQRY & cur_FF_Stampa_Dettagli_COD & ","

            Next


            GestioneConfig_PulisciOrfani(curId_agenda, NomeTabella_AgendaxUpdate, NomeColonna_AgendaxUpdate, objParametri_server)

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri_server.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server)

            End If


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)



        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)


        End Try

        Return rVal.TrimEnd(",")
    End Function

    Public Shared Sub getLayoutLinguaStampante(ByVal ConfigurazioneDaStampare As String, ByRef OModuli_Referenze_Config_Testata As Integer, ByRef id_agenda As Integer, ByRef id_mov_det As Integer, ByRef Layout_Cod As Integer, ByRef Stampante_cod As String, ByRef Stampante_Nome_PerStampa As String, ByRef lingua_cod As Integer, ByRef Numero_copie As Integer, ByRef FF_Stampanti_cod_FF_Stampa_Dettagli_COD As String, ByRef old_id_mov_det As Integer)

        Dim mpp() As String = ConfigurazioneDaStampare.Split(LabelPrintSeparator)

        If mpp.Length = 9 Then

            OModuli_Referenze_Config_Testata = mpp(0)
            id_agenda = mpp(1)
            id_mov_det = mpp(2)
            Layout_Cod = mpp(3)

            If mpp(4).Contains("-") Then
                Stampante_cod = mpp(4).Split("-")(0)
                old_id_mov_det = mpp(4).Split("-")(1)

            Else
                Stampante_cod = mpp(4)
            End If

            Stampante_Nome_PerStampa = mpp(5)
            lingua_cod = mpp(6)

            If mpp(7) = "" Then
                Numero_copie = 1
            Else
                Numero_copie = mpp(7)
            End If


            FF_Stampanti_cod_FF_Stampa_Dettagli_COD = mpp(8)


        End If

    End Sub

    Private Function GestioneConfig_PulisciOrfani(
            ByVal ID_Agenda As Integer,
            ByVal NomeTabella_AgendaxUpdate As String,
            ByVal NomeColonna_AgendaxUpdate As String,
            ByVal objParametri_server As AgronicaCoreParametri
        ) As Boolean


        Dim MessaggioErrore As String
        Dim NomeRoutine As String = "FF_GestoreConfigStampa.GestioneConfig"

        Dim scriviDet As New FF_Stampa_Dettagli_W
        Dim scriviCfg As New FF_ConfigurazioniDiStampa_Dettagli_W
        Dim sq As New Agro_Sequenze

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            'Se la connessione è chiusa la apro
            If objParametri_server.objConnessione Is Nothing Then
                'Richiedo una connessione
                objParametri_server.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri_server.StringaConnessione)
                objParametri_server.objConnessione.Open()
                FlagConnessioneLocale = True
            ElseIf objParametri_server.objConnessione.State = ConnectionState.Closed Then
                objParametri_server.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri_server.StringaConnessione)
                objParametri_server.objConnessione.Open()
                FlagConnessioneLocale = True
            End If

            If objParametri_server.objTransazione Is Nothing Then
                'Inizializzo la transazione
                objParametri_server.objTransazione = objParametri_server.objConnessione.BeginTransaction
                FlagTransazioneLocale = True
            End If





            scriviCfg.AggiornaOperazioneDiAgenda_PulisciOrfani(
                NomeTabella_AgendaxUpdate,
                NomeColonna_AgendaxUpdate,
                objParametri_server
            )

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            If FlagTransazioneLocale = True Then
                objParametri_server.objTransazione.Commit()
            End If

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri_server.objTransazione Is Nothing Then
                objParametri_server.objTransazione.Rollback()
                objParametri_server.objTransazione = Nothing
            End If

            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = ex.Message


            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

    End Function


    Private Function GestioneConfig_PulisciDettagli(
            ByVal ID_Agenda As Integer,
            ByVal NomeTabella_AgendaxUpdate As String,
            ByVal NomeColonna_AgendaxUpdate As String,
            ByVal objParametri_server As AgronicaCoreParametri
        ) As Boolean


        Dim MessaggioErrore As String
        Dim NomeRoutine As String = "FF_GestoreConfigStampa.GestioneConfig"

        Dim scriviDet As New FF_Stampa_Dettagli_W
        Dim scriviCfg As New FF_ConfigurazioniDiStampa_Dettagli_W
        Dim sq As New Agro_Sequenze

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            'Se la connessione è chiusa la apro
            If objParametri_server.objConnessione Is Nothing Then
                'Richiedo una connessione
                objParametri_server.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri_server.StringaConnessione)
                objParametri_server.objConnessione.Open()
                FlagConnessioneLocale = True
            ElseIf objParametri_server.objConnessione.State = ConnectionState.Closed Then
                objParametri_server.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri_server.StringaConnessione)
                objParametri_server.objConnessione.Open()
                FlagConnessioneLocale = True
            End If

            If objParametri_server.objTransazione Is Nothing Then
                'Inizializzo la transazione
                objParametri_server.objTransazione = objParametri_server.objConnessione.BeginTransaction
                FlagTransazioneLocale = True
            End If





            scriviCfg.AggiornaOperazioneDiAgenda(
                ID_Agenda,
                0,
                0,
                NomeTabella_AgendaxUpdate,
                NomeColonna_AgendaxUpdate,
                objParametri_server
            )

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            If FlagTransazioneLocale = True Then
                objParametri_server.objTransazione.Commit()
            End If

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri_server.objTransazione Is Nothing Then
                objParametri_server.objTransazione.Rollback()
                objParametri_server.objTransazione = Nothing
            End If

            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = ex.Message


            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

    End Function


    Private Function GestioneConfig(
        ByVal ID_Agenda As Integer,
        ByVal id_mov_det As Integer,
        ByRef FF_Stampa_Dettagli_COD As String,
        ByVal FF_LayoutEtichette_cod As Integer,
        ByVal FF_Stampanti_Cod As Integer,
        ByVal FF_Lingua_Cod As Integer,
        ByVal NomeTabella_AgendaxUpdate As String,
        ByVal NomeColonna_AgendaxUpdate As String,
        ByVal objParametri_server As AgronicaCoreParametri
    ) As Boolean



        Dim MessaggioErrore As String
        Dim NomeRoutine As String = "FF_GestoreConfigStampa.GestioneConfig"

        Dim scriviDet As New FF_Stampa_Dettagli_W
        Dim scriviCfg As New FF_ConfigurazioniDiStampa_Dettagli_W
        Dim sq As New Agro_Sequenze

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            'Se la connessione è chiusa la apro
            If objParametri_server.objConnessione Is Nothing Then
                'Richiedo una connessione
                objParametri_server.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri_server.StringaConnessione)
                objParametri_server.objConnessione.Open()
                FlagConnessioneLocale = True
            ElseIf objParametri_server.objConnessione.State = ConnectionState.Closed Then
                objParametri_server.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri_server.StringaConnessione)
                objParametri_server.objConnessione.Open()
                FlagConnessioneLocale = True
            End If

            If objParametri_server.objTransazione Is Nothing Then
                'Inizializzo la transazione
                objParametri_server.objTransazione = objParametri_server.objConnessione.BeginTransaction
                FlagTransazioneLocale = True
            End If


            If FF_Stampa_Dettagli_COD = 0 Then
                'FF_Stampa_Dettagli_COD = sq.Agronica_SequenzaTabelle_NuovoID("FF_Stampa_Dettagli", objParametri_server)
                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                FF_Stampa_Dettagli_COD = sq.NuovoId_Tabella("FF_Stampa_Dettagli", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_server)

                scriviDet.Scrivi( _
                    FF_Stampa_Dettagli_COD, _
                    "", _
                    objParametri_server _
                )

                scriviCfg.Scrivi( _
                    "", _
                    0, _
                    FF_Stampanti_Cod, _
                    FF_Lingua_Cod, _
                    FF_LayoutEtichette_cod, _
                    FF_Stampa_Dettagli_COD, _
                    objParametri_server _
                )

            Else
                scriviCfg.Cancella(FF_Stampa_Dettagli_COD, FF_Stampanti_Cod, "", objParametri_server)
                scriviCfg.Scrivi( _
                    "", _
                    0, _
                    FF_Stampanti_Cod, _
                    FF_Lingua_Cod, _
                    FF_LayoutEtichette_cod, _
                    FF_Stampa_Dettagli_COD, _
                    objParametri_server _
                )
            End If


            scriviCfg.AggiornaOperazioneDiAgenda( _
                ID_Agenda, _
                id_mov_det, _
                FF_Stampa_Dettagli_COD, _
                NomeTabella_AgendaxUpdate, _
                NomeColonna_AgendaxUpdate, _
                objParametri_server _
            )

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            If FlagTransazioneLocale = True Then
                objParametri_server.objTransazione.Commit()
            End If

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri_server.objTransazione Is Nothing Then
                objParametri_server.objTransazione.Rollback()
                objParametri_server.objTransazione = Nothing
            End If

            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = ex.Message


            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

    End Function

End Class
