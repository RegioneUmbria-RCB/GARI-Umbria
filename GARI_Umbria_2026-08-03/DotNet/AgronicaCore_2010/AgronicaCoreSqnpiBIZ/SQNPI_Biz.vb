

Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ

Imports AgronicaCoreDataProvider.ConnessioniTransazioni
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class SQNPI_Biz


    Private CuaaOP As String
    Private skOdc As String
    Private DataAdesione As String
    Private DestinatariResponso As String
    Private AnnoRiferimento As String

    Public Sub New(
        ByVal pCuaaOP As String, _
        ByVal pskOdc As String, _
        ByVal pDataAdesione As String, _
        ByVal pDestinatariResponso As String, _
        ByVal pAnnoRiferimento As String)

        CuaaOP = pCuaaOP
        skOdc = pskOdc
        DataAdesione = pDataAdesione
        DestinatariResponso = pDestinatariResponso
        AnnoRiferimento = pAnnoRiferimento

    End Sub
    Public Sub RimuoviPlanningDaCache(ByVal Stato_Richiesto As Integer, ByVal ws_SQNPI_IdFlusso_tot As String, ByVal objParametri_Server As AgronicaCoreParametri, ByRef Imposta_Stato_Complessivo_Flusso As Boolean, ByVal objParametri_Utenti As AgronicaCoreParametri)
        Dim oSQNPI_R As New AgronicaCoreSqnpiDAL.SQNPI_R
        Dim dtPlanningMemorizzatiSuCache_CUAA_DaRimuovere As DataTable = _
        oSQNPI_R.LeggiPlanningMemorizzatiSuCache_CUAA_DaRimuovere( _
            ws_SQNPI_IdFlusso_tot, _
            Stato_Richiesto, _
            enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_Pratica_acquisita_correttamente_nel_SQNPI, _
            "", _
            "", _
            objParametri_Server _
        )

        Dim dtPlanningMemorizzatiSuCache_CUAA_DaRimuovere_rows As DataRowCollection = dtPlanningMemorizzatiSuCache_CUAA_DaRimuovere.Rows

        For Each curSoggettoCUAA_DaRimuovere As DataRow In dtPlanningMemorizzatiSuCache_CUAA_DaRimuovere_rows
            RimuoviSoggettoCUAA(curSoggettoCUAA_DaRimuovere("SoggettoCUAA"), ws_SQNPI_IdFlusso_tot, objParametri_Server)
            Imposta_Stato_Complessivo_Flusso = True
        Next

    End Sub
    Public Function MemorizzaPlanningNonAncoraPresentiSuCache(ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As rispostaStandard


        Dim xRispostaComplessiva As New RispostaStandard
        xRispostaComplessiva.RispostaOK = True

        Dim NomeRoutine As String = "MemorizzaPlanningNonAncoraPresentiSuCache"

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        Try


            ApriConnessioneXCoreBiz( _
                FlagConnessioneLocale, _
                FlagTransazioneLocale, _
                objParametri_Server _
            )



            Dim oSQNPI_R As New AgronicaCoreSqnpiDAL.SQNPI_R


            'recupero la domanda che non si trova nello stato "acquisita_correttamente_nel_SQNPI"

            Dim dtXDomanda As DataTable
            dtXDomanda = oSQNPI_R.LeggiDomanda_PerAccodaDati(AnnoRiferimento, CuaaOP, enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_Pratica_acquisita_correttamente_nel_SQNPI, "", "", objParametri_Server)

            Dim Imposta_Stato_Complessivo_Flusso As Boolean = False

            Dim memDomanda As Boolean = False
            Dim ws_SQNPI_IdFlusso_tot As String = ""

            If dtXDomanda.Rows.Count > 0 Then
                ws_SQNPI_IdFlusso_tot = dtXDomanda.Rows(0)("IdentificativoFlusso")
            Else

                'solo se ci sono dei planning orfani di domanda, cioè da inviare, quindi da ricomprendere in una nuova domanda.
                Dim dtSQNPI_Orfani As DataTable = oSQNPI_R.LeggiPlanningNonMemorizzatiSuCache(ws_SQNPI_IdFlusso_tot, enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_Pratica_Validata, "", "", objParametri_Server)

                If dtSQNPI_Orfani.Rows.Count > 0 Then
                    Dim ws_SQNPI_IdFlusso As Integer
                    Dim oSequenzaTabelle As New Agro_Sequenze
                    'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                    ws_SQNPI_IdFlusso = oSequenzaTabelle.NuovoId_Tabella("ws_SQNPI_IdFlusso_" & AnnoRiferimento & "_" & CuaaOP, 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
                    'ws_SQNPI_IdFlusso = oSequenzaTabelle.Agronica_SequenzaTabelle_NuovoID("ws_SQNPI_IdFlusso_" & AnnoRiferimento & "_" & CuaaOP, objParametri_Server)
                    ws_SQNPI_IdFlusso_tot = AnnoRiferimento & "/" & CuaaOP & "/" & ws_SQNPI_IdFlusso.ToString
                    memDomanda = True
                End If


            End If


            '1a. Rimozione dei planning bloccati per dati non coerenti.        
            RimuoviPlanningDaCache( _
                enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_Bloccato_per_dati_non_coerenti, _
                ws_SQNPI_IdFlusso_tot, _
                objParametri_Server, _
                Imposta_Stato_Complessivo_Flusso, _
                objParametri_Utenti _
            )

            '1b. Rimozione dei planning in stato "validato", così saranno nuovamente inseriti.
            RimuoviPlanningDaCache( _
                enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_Pratica_Validata, _
                ws_SQNPI_IdFlusso_tot, _
                objParametri_Server, _
                Imposta_Stato_Complessivo_Flusso, _
                objParametri_Utenti _
            )


            '2. lista dei planning non ancora riportati su cache..
            Dim dtSQNPI As DataTable = oSQNPI_R.LeggiPlanningNonMemorizzatiSuCache(ws_SQNPI_IdFlusso_tot, enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_Pratica_Validata, "", "", objParametri_Server)


            Dim xRispostaStandardSingolo As RispostaStandard
            Dim dtSQNPI_Rows As DataRowCollection = dtSQNPI.Rows

            xRispostaComplessiva.RispostaStringa = "Planning Non Ancora Presenti Su Cache per domanda con identificativo flusso = " & ws_SQNPI_IdFlusso_tot & ": " & dtSQNPI_Rows.Count & " Planning trovati."



            For Each dtSQNPI_row As DataRow In dtSQNPI_Rows


                Dim Programmazione_cod As Integer = dtSQNPI_row("Programmazione_cod")
                xRispostaStandardSingolo = MemorizzaPlanning(memDomanda, CuaaOP, skOdc, DataAdesione, DestinatariResponso, Programmazione_cod, ws_SQNPI_IdFlusso_tot, objParametri_Server, objParametri_Utenti)

                xRispostaComplessiva.RispostaOK = (xRispostaComplessiva.RispostaOK And xRispostaStandardSingolo.RispostaOK)
                xRispostaComplessiva.RispostaStringa &= "Planning ID = " & Programmazione_cod.ToString & ": " & xRispostaStandardSingolo.RispostaStringa & vbCrLf
                xRispostaComplessiva.Errore &= "Planning ID = " & Programmazione_cod.ToString & ": " & xRispostaStandardSingolo.Errore & vbCrLf

                If xRispostaStandardSingolo.RispostaOK Then
                    Imposta_Stato_Complessivo_Flusso = True
                End If

            Next


            If Imposta_Stato_Complessivo_Flusso Then

                Dim oSQPNI As New AgronicaCoreSqnpiDAL.SQNPI_W
                oSQPNI.AvanzamentoDiStato_Planning_dato_IdentificativoFlusso(ws_SQNPI_IdFlusso_tot, enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_In_fase_di_invio, objParametri_Server)
                oSQPNI.AvanzamentoDiStato(ws_SQNPI_IdFlusso_tot, "ws_SQNPI_Domanda", enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_In_fase_di_invio, 0, objParametri_Server)

            End If

            ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception

            'rollback
            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ChiudiTransazione(2, objParametri_Server)

            End If

            xRispostaComplessiva.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            xRispostaComplessiva.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)

            ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & xRispostaComplessiva.Errore, ex)

        Finally

            ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try


        Return xRispostaComplessiva

    End Function



    Private Function Impostazione(ByVal Cod_impostazione As Integer, ByVal objParametri_Utenti As AgronicaCoreParametri) As String

        Dim rVal As String = ""
        Dim dtImpostazione As DataTable

        Dim oLetturaImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        dtImpostazione = oLetturaImpostazioni.Leggi(Cod_impostazione, 2, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

        If dtImpostazione.Rows.Count > 0 Then
            rVal = dtImpostazione.Rows(0)("Impostazione_Valore_1")
        Else
            rVal = "-1"
        End If

        Return rVal
    End Function



    ''' <summary>
    ''' Accoda un planning ad flusso per l'invio dei dati al web service SQNPI.
    ''' </summary>
    ''' <param name="MemorizzaFlusso"></param>
    ''' <param name="pCuaaOP"></param>
    ''' <param name="pskOdc"></param>
    ''' <param name="pDataAdesione"></param>
    ''' <param name="pDestinatariResponso"></param>
    ''' <param name="Programmazione_Cod"></param>
    ''' <param name="IdentificativoFlusso"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function MemorizzaPlanning(
        ByRef MemorizzaFlusso As Boolean, _
        ByVal pCuaaOP As String, _
        ByVal pskOdc As String, _
        ByVal pDataAdesione As String, _
        ByVal pDestinatariResponso As String,
        ByVal Programmazione_Cod As Integer, _
        ByVal IdentificativoFlusso As String, _
        ByVal objParametri_Server As AgronicaCoreParametri, _
        ByVal objParametri_Utenti As AgronicaCoreParametri) As rispostaStandard

        Dim NomeRoutine As String = "MemorizzaPlanning"

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        Dim oSQPNI As New AgronicaCoreSqnpiDAL.SQNPI_W

        Dim rs As New RispostaStandard

        Dim MessaggioFinale As String = "Operazione eseguita correttamente."

        Try


            Dim Filtro_Veg_cod As String = ""
            Dim Filtro_Macrouso_cod As String = ""
            Dim Filtro_id_cod As String = ""

            Filtro_Veg_cod = Impostazione(enum_Impostazioni_Utenti.SUPERUSER_COD_SQPNI_FILTROSPECIE, objParametri_Utenti)
            Filtro_Macrouso_cod = Impostazione(enum_Impostazioni_Utenti.SUPERUSER_COD_SQPNI_FILTROMACRO_USI, objParametri_Utenti)
            Filtro_id_cod = Impostazione(enum_Impostazioni_Utenti.SUPERUSER_COD_SQPNI_FILTRODESTINAZIONE_USO, objParametri_Utenti)


            'apro la transazione

            'Apro la connessione al DB
            ApriConnessioneXCoreBiz( _
                FlagConnessioneLocale, _
                FlagTransazioneLocale, _
                objParametri_Server _
            )


            If MemorizzaFlusso Then
                oSQPNI.MemorizzaDomanda(IdentificativoFlusso, CuaaOP, skOdc, DataAdesione, DestinatariResponso, objParametri_Server)
                MemorizzaFlusso = False
            End If

            oSQPNI.MemorizzaSoggetto(IdentificativoFlusso, Programmazione_Cod, objParametri_Server)

            oSQPNI.Memorizza_Domanda_Soggetto_Programmazione_Testata(IdentificativoFlusso, Programmazione_Cod, objParametri_Server)

            oSQPNI.MemorizzaPlanning(IdentificativoFlusso, Programmazione_Cod, Filtro_Veg_cod, Filtro_Macrouso_cod, Filtro_id_cod, objParametri_Server)

            oSQPNI.AvanzamentoDiStato_Planning(Programmazione_Cod, enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_In_fase_di_invio, objParametri_Server)

            rs.RispostaStringa = MessaggioFinale
            rs.RispostaOK = True


            'chiudo
            ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception

            'rollback
            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ChiudiTransazione(2, objParametri_Server)

            End If

            rs.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            rs.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)

            ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & rs.Errore, ex)

        Finally

            ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return rs

    End Function



    ''' <summary>
    ''' Rimozione di un soggetto dalla cache
    ''' </summary>
    ''' <param name="SoggettoCUAA"></param>
    ''' <param name="IdentificativoFlusso"></param>
    ''' <param name="objParametri_Server"></param>    
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function RimuoviSoggettoCUAA(
        ByVal SoggettoCUAA As String, _
        ByVal IdentificativoFlusso As String, _
        ByVal objParametri_Server As AgronicaCoreParametri _
    ) As rispostaStandard

        Dim NomeRoutine As String = "MemorizzaPlanning"

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        Dim oSQPNI As New AgronicaCoreSqnpiDAL.SQNPI_W

        Dim rs As New RispostaStandard

        Dim MessaggioFinale As String = "Operazione eseguita correttamente."

        Try


            'apro la transazione

            'Apro la connessione al DB
            ApriConnessioneXCoreBiz( _
                FlagConnessioneLocale, _
                FlagTransazioneLocale, _
                objParametri_Server _
            )


            oSQPNI.RimuoviSoggettoTerra(IdentificativoFlusso, SoggettoCUAA, objParametri_Server)
            oSQPNI.RimuoviSoggettoPlan(IdentificativoFlusso, SoggettoCUAA, objParametri_Server)
            oSQPNI.RimuoviSoggetto(IdentificativoFlusso, SoggettoCUAA, objParametri_Server)



            rs.RispostaStringa = MessaggioFinale
            rs.RispostaOK = True


            'chiudo
            ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception

            'rollback
            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ChiudiTransazione(2, objParametri_Server)

            End If

            rs.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            rs.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)

            ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & rs.Errore, ex)

        Finally

            ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return rs

    End Function


End Class
