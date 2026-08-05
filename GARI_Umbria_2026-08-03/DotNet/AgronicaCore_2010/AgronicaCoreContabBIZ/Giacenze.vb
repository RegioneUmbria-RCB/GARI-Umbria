Imports System.Data
Imports System.Xml
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider
Imports System.Data.Common
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD

Public Class Giacenze_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '============================================================================
    Public Function Giacenza_Scrivi(ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Elem_Cod As Integer,
                                    ByVal Pro_Cod As Integer,
                                    ByVal Mat_Cod As Integer,
                                    ByVal Udm_Cod As Integer,
                                    ByVal Id_Destinazione As Integer,
                                    ByVal Operazione As String,
                                    ByVal Cau_Mov As String,
                                    ByVal Cal_Cod As Integer,
                                    ByVal Cod_Progetto As Integer,
                                    ByVal Fase_Cod As Integer,
                                    ByVal Lotto As String,
                                    ByVal Qta As Decimal,
                                    ByVal Prezzo_Unitario As Decimal,
                                    ByVal Udm_Cod_Extra As Integer,
                                    ByVal Qta_Extra As Decimal,
                                    ByVal Qta_Extra_Totale As Decimal,
                                    ByVal Variazione As Decimal,
                                    ByVal Lav_Cod As Integer,
                                    ByVal Jolly_Int As Integer,
                                    ByVal BaseCode As Integer,
                                    ByVal TopCode As Integer,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        '----------------------------------------------------------------------

        Const nomeRoutine = "AgronicaCoreContabBIZ.Giacenze_W.Giacenza_Scrivi()"

        'Lav_Cod = -1
        'Tipo_Destinazione = 20 (Magazzino)

        '----------------------------------------------------------------------

        Dim objMovimentiDettagliR As AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim objMovimentiDettagliW As AgronicaCoreContabDAL.Movimenti_Dettagli_W
        Dim objMovimenti As AgronicaCoreContabDAL.Movimenti_W
        Dim objMovDestinazioni As AgronicaCoreContabDAL.Mov_Destinazioni_W
        Dim objAgenda As AgronicaCoreContabDAL.Agenda_W
        Dim objSequenze As Agro_Sequenze

        Dim dtGiacenze As DataTable

        Dim Dummy As Object

        Dim Id_Agenda As Integer
        Dim Id_Mov As Integer
        Dim Id_Mov_Det As Integer
        Dim Id_Mov_Det_Des As Integer

        Dim mQta As Decimal
        Dim mQta_Dettaglio As Decimal

        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------


        Try

            '-------------------------------------------------------------------------------

            objMovimentiDettagliR = New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            objMovimentiDettagliW = New AgronicaCoreContabDAL.Movimenti_Dettagli_W
            objMovDestinazioni = New AgronicaCoreContabDAL.Mov_Destinazioni_W


            '===============================================================================
            'Verifica dell'esistenza dell'Id_Agenda relativo le giacenze di magazzino
            '-------------------------------------------------------------------------------

            dtGiacenze = objMovimentiDettagliR.LeggiGiacenze_Chiavi(
                                    CStr(Piva),
                                    CInt(Sa_Cod),
                                    CInt(Elem_Cod),
                                    CInt(Pro_Cod),
                                    CInt(Mat_Cod),
                                    CInt(Udm_Cod),
                                    CInt(Cal_Cod),
                                    CInt(Cod_Progetto),
                                    CInt(Fase_Cod),
                                    CStr(Lotto),
                                    CInt(Id_Destinazione),
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "",
                                    "",
                                    objParametri)


            'Questa Query ritorna le chiavi (Piva,Id_Agenda,Id_Mov,Id_Mov_Det,Id_Mov_Det_Des) con il suffisso '_Chiave'.
            'Il campo 'Id_Mov_Det_Des_Chiave' identifica se esiste o meno il record dentro la tabella 'Mov_Destinazioni'.
            'Potrebbe facilmente capitare che esista il record delle giacenze del prodotto dentro la tabella 'Movimenti_Dettagli'
            'ma non sia mai stato caricato dentro il magazzino ('Id_Destinazione')

            'Se lo stato del recordset è = 0 --> primo inserimento --> creazione records nelle tabelle 'Agenda' e 'Movimenti'.

            'Il resto del codice è elementare.

            Select Case dtGiacenze.Rows.Count

                Case 0 ' SALVA --------------------------------------------------------

                    'Creazione dell'Id_Agenda e del movimento relativo le giacenze

                    objSequenze = New Agro_Sequenze
                    objAgenda = New AgronicaCoreContabDAL.Agenda_W
                    objMovimenti = New AgronicaCoreContabDAL.Movimenti_W

                    'Richiedo un nuovo codice AGENDA

                    Id_Agenda = objSequenze.NuovoId_Tabella("Agenda",
                                                            BaseCode,
                                                            TopCode,
                                                            objParametri)


                    Dummy = objAgenda.Scrivi(CStr(Piva),
                                             0,
                                             CInt(Id_Agenda),
                                             -1,
                                             0,
                                             0,
                                             0,
                                             "Gestione Giacenze di Magazzino",
                                             0,
                                             0,
                                             "",
                                             AGRODATAINIZIO,
                                             Validita_Inizio,
                                             Validita_Fine,
                                             objParametri)

                    'Richiedo un nuovo codice MOVIMENTO
                    Id_Mov = objSequenze.NuovoId_Tabella("Movimenti",
                                                        CInt(BaseCode),
                                                        CInt(TopCode),
                                                        objParametri)


                    Dummy = objMovimenti.Scrivi(CStr(Piva),
                                                0,
                                                CInt(Id_Agenda),
                                                CInt(Id_Mov),
                                                0,
                                                "GIACENZE",
                                                "Rilevamento Giacenze",
                                                CDate(Date.Now),
                                                CDate(Date.Now),
                                                CDate(Date.Now),
                                                0, 0,
                                                0, 0, 0, 0, 0, 0, "", "",
                                                0,
                                                AGRODATAINIZIO,
                                                0, 0, "", 0,
                                                AGRODATAINIZIO,
                                                "", "", "", 0, 0, 0, 0, "",
                                                0, 0,
                                                AGRODATAINIZIO,
                                                0, 0,
                                                Validita_Inizio,
                                                Validita_Fine,
                                                0,
                                                objParametri)

                    Id_Mov_Det = 0 'Inesistente
                    Id_Mov_Det_Des = 0 'Inesistente


                    'Crashing Oggetti
                    objSequenze = Nothing
                    objAgenda = Nothing
                    objMovimenti = Nothing


                Case Else ' LETTURA ---------------------------------------------------------

                    'Id_Agenda e id_Movimento Individuati

                    '=======================================================================
                    'Impostazione Parametri
                    '-----------------------------------------------------------------------
                    Id_Agenda = dtGiacenze.Rows(0).Item("Id_Agenda_Chiave")
                    Id_Mov = dtGiacenze.Rows(0).Item("Id_Mov_Chiave")
                    Id_Mov_Det = dtGiacenze.Rows(0).Item("Id_Mov_Det_Chiave")
                    Id_Mov_Det_Des = dtGiacenze.Rows(0).Item("Id_Mov_Det_Des_Chiave")
                    '=======================================================================

            End Select


            '=====================================================================================
            'Verifica dell'esistenza dell'Id_Mov_Det relativo le giacenza del prodotto
            '-------------------------------------------------------------------------------------

            Select Case Id_Mov_Det

                Case 0 ' SALVA ---------------------------------------------------------------


                    'Creazione dell'Id_Mov_Det relativo le giacenza del prodotto

                    'Richiedo un nuovo codice movimento_dettaglio
                    objSequenze = New Agro_Sequenze

                    Id_Mov_Det = objSequenze.NuovoId_Tabella("Movimenti_Dettagli",
                                                             CInt(BaseCode),
                                                             CInt(TopCode),
                                                             objParametri)
                    objSequenze = Nothing


                    Select Case Cau_Mov

                        Case "7300", "4100", "7900", "7920" 'Carico, Conferimento da Soci, Accettazioni

                            'Qta = Qta

                        Case "7350", "4200" 'Scarico, Conferimento a Diversi

                            'Il primo movimento è uno scarico
                            Qta = -Qta

                    End Select


                    Dummy = objMovimentiDettagliW.Scrivi(CStr(Piva),
                                                        CInt(Sa_Cod),
                                                        CInt(Id_Agenda),
                                                        CInt(Id_Mov),
                                                        CInt(Id_Mov_Det),
                                                        CInt(Elem_Cod),
                                                        CInt(Pro_Cod),
                                                        CInt(Mat_Cod),
                                                        "Rilevamento Giacenza del " & Format(CDate(Now), "dd/MM/yyyy"),
                                                        CDbl(Qta),
                                                        CInt(Udm_Cod),
                                                        0, 0, 0,
                                                        CDbl(Prezzo_Unitario),
                                                        CDbl(Prezzo_Unitario),
                                                        0, CInt(Cal_Cod),
                                                        CInt(Cod_Progetto),
                                                        CInt(Fase_Cod),
                                                        "", 0, CDate(Now),
                                                        0, 0, 0, 0, 0, 0,
                                                        1, 1, CStr(Lotto),
                                                        CInt(Udm_Cod_Extra),
                                                        CDbl(Qta_Extra),
                                                        CDbl(Qta_Extra_Totale),
                                                        0, CDbl(Variazione),
                                                        0, 0, 0, 0,
                                                        0, "",
                                                        0, 0, 0,
                                                        "", "", "",
                                                        Validita_Inizio,
                                                        Validita_Fine,
                                                        objParametri)
                    '
                Case Else 'MODIFICA -------------------------------------------------------


                    '================================================================================
                    'Aggiornamento delle Quantità presenti nel Magazzino.
                    '--------------------------------------------------------------------------------
                    If ((Cau_Mov = "7300" Or Cau_Mov = "4100" Or Cau_Mov = "7900" Or Cau_Mov = "7920") And Operazione = "1") Or
                    ((Cau_Mov = "7350" Or Cau_Mov = "4200") And Operazione = "3") Then


                        'Inserimento carico o cancellazione scarico

                        '===========================================================================
                        'Gestione Giacenze
                        If Jolly_Int = 0 Then
                            'Il movimento ricade sulla gestione giacenze
                            mQta_Dettaglio = CDbl(dtGiacenze.Rows(0).Item("Dett_Qta")) + Qta 'Quantità Attuale
                        Else
                            'Il movimento non ricade sulla gestione giacenze
                            mQta_Dettaglio = CDbl(dtGiacenze.Rows(0).Item("Dett_Qta")) 'Quantità invariata
                        End If


                    ElseIf ((Cau_Mov = "7300" Or Cau_Mov = "4100" Or Cau_Mov = "7900" Or Cau_Mov = "7920") And Operazione = "3") Or
                        ((Cau_Mov = "7350" Or Cau_Mov = "4200") And Operazione = "1") Then


                        'Inserimento scarico o cancellazione carico

                        '===========================================================================
                        'Gestione Giacenze
                        If Jolly_Int = 0 Then
                            'Il movimento ricade sulla gestione giacenze
                            mQta_Dettaglio = CDbl(dtGiacenze.Rows(0).Item("Dett_Qta")) - Qta 'Quantità Attuale
                        Else
                            'Il movimento non ricade sulla gestione giacenze
                            mQta_Dettaglio = CDbl(dtGiacenze.Rows(0).Item("Dett_Qta")) 'Quantità invariata
                        End If


                    End If



                    'Modifica della giacenza del prodotto
                    objMovimentiDettagliW.Modifica(CStr(Piva), CInt(Sa_Cod),
                                                   Id_Agenda, Id_Mov,
                                                   Id_Mov_Det,
                                                   CInt(Elem_Cod), CInt(Pro_Cod),
                                                   CInt(Mat_Cod),
                                                   "Rilevamento Giacenze del " & Format(CDate(Now), "dd/MM/yyyy"),
                                                   CDbl(mQta_Dettaglio),
                                                   CInt(Udm_Cod), 0, 0,
                                                   0,
                                                   0,
                                                   0,
                                                   CInt(Cal_Cod), 0,
                                                   CInt(Cod_Progetto),
                                                   CInt(Fase_Cod),
                                                   "", 0, CDate(Now),
                                                   0, 0, 0, 0, 0,
                                                   1, 1, CStr(Lotto),
                                                   CInt(Udm_Cod_Extra),
                                                   CDbl(Qta_Extra),
                                                   CDbl(Qta_Extra_Totale), 0, 0, 0, 0, 0,
                                                   0, "",
                                                   0, 0, 0,
                                                   Validita_Inizio,
                                                   Validita_Fine,
                                                   "",
                                                   objParametri)

            End Select

            'Crashing Oggetti
            objMovimentiDettagliW = Nothing

            '=====================================================================================

            '=====================================================================================
            'Verifica dell'esistenza del riferimento al magazzino del prodotto
            '------------------------------------------------------------------------------------

            Select Case Id_Mov_Det_Des

                Case 0 'SALVA ----------------------------------------------------------------

                    'Salvo la destinazione
                    Dummy = objMovDestinazioni.Scrivi(CStr(Piva),
                                                      CInt(Sa_Cod),
                                                      CInt(Id_Agenda),
                                                      CInt(Id_Mov),
                                                      CInt(Id_Mov_Det),
                                                      0,
                                                      CInt(Id_Destinazione),
                                                      20,
                                                      CDbl(Qta),
                                                      0,
                                                      0,
                                                      0,
                                                      "",
                                                      0,
                                                      Validita_Inizio,
                                                      Validita_Fine,
                                                      objParametri)

                Case Else 'MODIFICA -----------------------------------------------------------

                    '================================================================================
                    'Aggiornamento delle Quantità presenti nel Magazzino.
                    '--------------------------------------------------------------------------------
                    If ((Cau_Mov = "7300" Or Cau_Mov = "4100" Or Cau_Mov = "7900" Or Cau_Mov = "7920") And Operazione = "1") Or
                    ((Cau_Mov = "7350" Or Cau_Mov = "4200") And Operazione = "3") Then


                        'Inserimento carico o cancellazione scarico

                        '===========================================================================
                        'Gestione Giacenze
                        If Jolly_Int = 0 Then
                            'Il movimento ricade sulla gestione giacenze
                            mQta = CDbl(dtGiacenze.Rows(0).Item("Qta")) + Qta 'Quantità Attuale
                        Else
                            'Il movimento non ricade sulla gestione giacenze
                            mQta = CDbl(dtGiacenze.Rows(0).Item("Qta")) 'Quantità invariata
                        End If


                    ElseIf ((Cau_Mov = "7300" Or Cau_Mov = "4100" Or Cau_Mov = "7900" Or Cau_Mov = "7920") And Operazione = "3") Or
                        ((Cau_Mov = "7350" Or Cau_Mov = "4200") And Operazione = "1") Then


                        'Inserimento scarico o cancellazione carico

                        '===========================================================================
                        'Gestione Giacenze
                        If Jolly_Int = 0 Then
                            'Il movimento ricade sulla gestione giacenze
                            mQta = CDbl(dtGiacenze.Rows(0).Item("Qta")) - Qta 'Quantità Attuale
                        Else
                            'Il movimento non ricade sulla gestione giacenze
                            mQta = CDbl(dtGiacenze.Rows(0).Item("Qta")) 'Quantità invariata
                        End If


                    End If

                    '==========================================================================

                    objMovDestinazioni.Modifica(CStr(Piva),
                                                CInt(Sa_Cod),
                                                CInt(Id_Agenda),
                                                CInt(Id_Mov),
                                                CInt(Id_Mov_Det),
                                                0,
                                                CInt(Id_Destinazione),
                                                20,
                                                CDbl(mQta),
                                                0,
                                                "",
                                                0,
                                                Validita_Inizio,
                                                Validita_Fine,
                                                "",
                                                objParametri)

            End Select

            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            dtGiacenze.Dispose()
            dtGiacenze = Nothing
            objMovDestinazioni = Nothing

            '------------------------------

            'Restituisco un valore Dummy
            xRisp = True

            'Se la transazione è stata avviata in questa routine faccio il commit
            If flagTransazioneLocale = True Then
                objParametri.objTransazione.Commit()
            End If

            '----------------------------------------------------------------------------

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                objParametri.objTransazione.Rollback()
                objParametri.objTransazione = Nothing
            End If

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            If (flagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
                objParametri.objConnessione.Close()
            End If

        End Try

        Return xRisp


    End Function



    '============================================================================
    Public Function ConsistenzeEnologiche_Scrivi(ByVal Piva As String,
                                                 ByVal Sa_Cod As Integer,
                                                 ByVal Elem_Cod As Integer,
                                                 ByVal Pro_Cod As Integer,
                                                 ByVal Mat_Cod As Integer,
                                                 ByVal Udm_Cod As Integer,
                                                 ByVal Id_Destinazione As Integer,
                                                 ByVal Operazione As String,
                                                 ByVal Cau_Mov As String,
                                                 ByVal Cal_Cod As Integer,
                                                 ByVal Cod_Progetto As Integer,
                                                 ByVal Fase_Cod As Integer,
                                                 ByVal Lotto As String,
                                                 ByVal Qta As Decimal,
                                                 ByVal Prezzo_Unitario As Decimal,
                                                 ByVal Udm_Cod_Extra As Integer,
                                                 ByVal Qta_Extra As Decimal,
                                                 ByVal Qta_Extra_Totale As Decimal,
                                                 ByVal Variazione As Decimal,
                                                 ByVal Lav_Cod As Integer,
                                                 ByVal Jolly_Int As Integer,
                                                 ByVal BaseCode As Integer,
                                                 ByVal TopCode As Integer,
                                                 ByVal Validita_Inizio As Date,
                                                 ByVal Validita_Fine As Date,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As Boolean

        '----------------------------------------------------------------------

        Const nomeRoutine = "AgronicaCoreContabBIZ.Giacenze_W.ConsistenzeEnologiche_Scrivi()"

        'Lav_Cod = -3
        'Tipo_Destinazione = 13 (Vasca Enologica)

        '----------------------------------------------------------------------

        Dim objMovimentiDettagliR As AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim objMovimentiDettagliW As AgronicaCoreContabDAL.Movimenti_Dettagli_W
        Dim objMovimenti As AgronicaCoreContabDAL.Movimenti_W
        Dim objMovDestinazioni As AgronicaCoreContabDAL.Mov_Destinazioni_W
        Dim objAgenda As AgronicaCoreContabDAL.Agenda_W
        Dim objSequenze As Agro_Sequenze

        Dim dtConsistenzeEno As DataTable

        Dim dummy As Boolean

        Dim Cod_Agenda As Integer
        Dim Cod_Movimento As Integer
        Dim Cod_Movimento_Dettaglio As Integer

        Dim mQta As Decimal
        Dim mStoricoCarico As Decimal


        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------


        Try

            '-------------------------------------------------------------------------------

            '===============================================================================
            'Verifica dell'esistenza dell'Id_Agenda relativo le Consistenze Eno
            '-------------------------------------------------------------------------------

            objMovimentiDettagliR = New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            objAgenda = New AgronicaCoreContabDAL.Agenda_W
            objMovimenti = New AgronicaCoreContabDAL.Movimenti_W


            'Lettura dell'Id_Agenda delle Consistenze Eno
            dtConsistenzeEno = objMovimentiDettagliR.LeggiConsistenzeEnologiche(CStr(Piva),
                                                                                0,
                                                                                0,
                                                                                0,
                                                                                0,
                                                                                0,
                                                                                0,
                                                                                0,
                                                                                0,
                                                                                0,
                                                                                "",
                                                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                "",
                                                                                "",
                                                                                objParametri)


            'Select Case RsConsistenzeEno.State
            Select Case dtConsistenzeEno.Rows.Count

                Case 0 ' SALVA --------------------------------------------------------

                    'Creazione dell'Id_Agenda e del movimento relativo le Consistenze Eno

                    objSequenze = New Agro_Sequenze

                    'Richiedo un nuovo codice AGENDA
                    Cod_Agenda = objSequenze.NuovoId_Tabella("Agenda",
                                                            CInt(BaseCode),
                                                            CInt(TopCode),
                                                            objParametri)

                    dummy = objAgenda.Scrivi(CStr(Piva),
                                             0,
                                             CInt(Cod_Agenda),
                                             -3,
                                             0,
                                             0,
                                             0,
                                             "Gestione Consistenze Enologiche in Vasca",
                                             0,
                                             0,
                                             "",
                                             AGRODATAINIZIO,
                                             Validita_Inizio,
                                             Validita_Fine,
                                             objParametri)


                    'Richiedo un nuovo codice MOVIMENTO
                    Cod_Movimento = objSequenze.NuovoId_Tabella("Movimenti",
                                                                CInt(BaseCode),
                                                                CInt(TopCode),
                                                                objParametri)


                    dummy = objMovimenti.Scrivi(CStr(Piva),
                                                0,
                                                CInt(Cod_Agenda),
                                                CInt(Cod_Movimento),
                                                0,
                                                "MOSTO",
                                                "Rilevamento Consistenze Enologiche in Vasca",
                                                CDate(Date.Now),
                                                CDate(Date.Now),
                                                CDate(Date.Now),
                                                0, 0,
                                                0, 0, 0, 0, 0, 0, "", "",
                                                0,
                                                AGRODATAINIZIO,
                                                0, 0, "", 0,
                                                AGRODATAINIZIO,
                                                "", "", "", 0, 0, 0, 0, "", 0, 0,
                                                AGRODATAINIZIO, 0, 0,
                                                Validita_Inizio,
                                                Validita_Fine,
                                                0,
                                                objParametri)

                    objSequenze = Nothing


                Case Else ' LETTURA ---------------------------------------------------------

                    'Id_Agenda e id_Movimento Individuato

                    '=======================================================================
                    'Impostazione Parametri
                    '-----------------------------------------------------------------------
                    Cod_Agenda = dtConsistenzeEno.Rows(0).Item("Id_Agenda")
                    Cod_Movimento = dtConsistenzeEno.Rows(0).Item("Id_Mov")
                    '=======================================================================

            End Select

            objAgenda = Nothing
            objMovimenti = Nothing
            dtConsistenzeEno.Dispose()
            dtConsistenzeEno = Nothing

            '=====================================================================================



            '=====================================================================================
            'Verifica dell'esistenza dell'Id_Movimento_Dettaglio relativo le Consistenze Eno del prodotto
            '-------------------------------------------------------------------------------------

            objMovimentiDettagliW = New AgronicaCoreContabDAL.Movimenti_Dettagli_W

            dtConsistenzeEno = objMovimentiDettagliR.LeggiConsistenzeEnologiche(CStr(Piva),
                                                                                CStr(Sa_Cod),
                                                                                CInt(Elem_Cod),
                                                                                CInt(Pro_Cod),
                                                                                CInt(Mat_Cod),
                                                                                CInt(Udm_Cod),
                                                                                0,
                                                                                CInt(Cal_Cod),
                                                                                CInt(Cod_Progetto),
                                                                                CInt(Fase_Cod),
                                                                                CStr(Lotto),
                                                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                "",
                                                                                "",
                                                                                objParametri)

            'Select Case RsConsistenzeEno.State
            Select Case dtConsistenzeEno.Rows.Count

                Case 0 ' SALVA ---------------------------------------------------------------


                    'Creazione dell'Cod_Movimento_Dettaglio relativo le Consistenza Eno del prodotto

                    'Richiedo un nuovo codice movimento_dettaglio

                    objSequenze = New Agro_Sequenze

                    Cod_Movimento_Dettaglio = objSequenze.NuovoId_Tabella("Movimenti_Dettagli",
                                                                          CInt(BaseCode),
                                                                          CInt(TopCode),
                                                                          objParametri)

                    objSequenze = Nothing


                    Select Case Cau_Mov

                        Case "7300", "4100", "7900", "7920" 'Carico, Conferimento da Soci, Accettazioni

                            'Qta = Qta

                        Case "7350", "4200" 'Scarico, Conferimento a Diversi

                            'Il primo movimento è uno scarico
                            Qta = -Qta

                    End Select


                    dummy = objMovimentiDettagliW.Scrivi(CStr(Piva), CInt(Sa_Cod),
                                                        CInt(Cod_Agenda), CInt(Cod_Movimento),
                                                        CInt(Cod_Movimento_Dettaglio),
                                                        CInt(Elem_Cod),
                                                        CInt(Pro_Cod), CInt(Mat_Cod),
                                                        "Rilevamento Consistenza Enologica del " & Format(CDate(Now), "dd/MM/yyyy"),
                                                        CDbl(Qta), CInt(Udm_Cod),
                                                        0, 0, CDbl(Qta),
                                                        CDbl(Prezzo_Unitario), CDbl(Prezzo_Unitario), 0,
                                                        CInt(Cal_Cod),
                                                        CInt(Cod_Progetto), CInt(Fase_Cod),
                                                        "", 0, CDate(Now),
                                                        0, 0, 0, 0, 0, 0,
                                                        1, 1, CStr(Lotto),
                                                        CInt(Udm_Cod_Extra), CDbl(Qta_Extra),
                                                        CDbl(Qta_Extra_Totale), 0,
                                                        CDbl(Variazione), 0, 0, 0, 0,
                                                        0, "",
                                                        0, 0, 0,
                                                        "", "", "",
                                                        Validita_Inizio,
                                                        Validita_Fine,
                                                        objParametri)
                    '
                Case Else 'MODIFICA -------------------------------------------------------

                    '=======================================================================
                    'Impostazione Parametri
                    '-----------------------------------------------------------------------
                    Cod_Movimento_Dettaglio = dtConsistenzeEno.Rows(0).Item("Id_Mov_Det")
                    '=======================================================================

                    '==========================================================================
                    'Aggiornamento delle Quantità e della Valorizzazione della Consistenza Eno.

                    'Calcolo della Valorizzazione Media Ponderata della Consistenza Eno.
                    '--------------------------------------------------------------------------
                    Select Case Cau_Mov

                        Case "7300", "4100" 'Carico, Conferimento da Soci

                            If Operazione = "1" Then

                                'Inserimento di uno nuovo carico:

                                '===========================================================================
                                'Gestione Consistenze Eno
                                If Jolly_Int = 0 Then
                                    'Il movimento ricade sulla gestione Consistenza Eno
                                    mQta = CDbl(dtConsistenzeEno.Rows(0).Item("Dett_Qta")) + Qta 'Quantità Attuale
                                Else
                                    'Il movimento non ricade sulla gestione Consistenza Eno
                                    mQta = CDbl(dtConsistenzeEno.Rows(0).Item("Dett_Qta")) 'Quantità invariata
                                End If

                                '===========================================================================
                                'Gestione Valorizzazione Economica
                                Select Case Lav_Cod
                                    Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_CONFERIMENTO, LAVCOD_TRASFERIMENTO, LAVCOD_CONFERIMENTO_DIVERSI '1025 Bolle + 1050 Conferimenti + 1033 Trasferimenti + 1052 LAVCOD_CONFERIMENTO_DIVERSI + 1031 LAVCOD_BOLLA_EMESSA
                                        mStoricoCarico = CDbl(dtConsistenzeEno.Rows(0).Item("Sconto")) 'Valorizzazione Invariata
                                    Case Else
                                        mStoricoCarico = CDbl(dtConsistenzeEno.Rows(0).Item("Sconto")) + Qta 'Valorizzazione Alterata
                                End Select


                                'Evito la Division By zero
                                If mStoricoCarico = 0 Then
                                    Prezzo_Unitario = 0

                                Else

                                    ' Aggiornamento dello storico dei carichi del prodotto e determinazione della media ponderata
                                    Prezzo_Unitario = Format(CDbl(CDbl(Prezzo_Unitario * Qta) + CDbl(dtConsistenzeEno.Rows(0).Item("Prezzo_Unitario") * dtConsistenzeEno.Rows(0).Item("Sconto"))) / (mStoricoCarico), "##,###,##0.00")

                                End If
                                '===========================================================================


                            ElseIf Operazione = "3" Then

                                'Cancellazione di un carico

                                '===========================================================================
                                'Gestione Consistenza Eno
                                If Jolly_Int = 0 Then
                                    'Il movimento ricade sulla gestione Consistenza Eno
                                    mQta = CDbl(dtConsistenzeEno.Rows(0).Item("Dett_Qta")) - Qta 'Quantità Attuale
                                Else
                                    'Il movimento non ricade sulla gestione Consistenza Eno
                                    mQta = CDbl(dtConsistenzeEno.Rows(0).Item("Dett_Qta")) 'Quantità invariata
                                End If


                                'Valorizzazione Economica Sospesa

                                '''                      '===========================================================================
                                '''                      'Gestione Valorizzazione Economica
                                '''                      Select Case Lav_Cod
                                '''                         Case 1025, 1050, 1031, 1052: 'Bolle + Conferimenti
                                '''                              mStoricoCarico = CDbl(RsConsistenzeEno("Sconto")) 'Valorizzazione Invariata
                                '''                         Case Else
                                '''                              mStoricoCarico = CDbl(RsConsistenzeEno("Sconto")) - Qta 'Valorizzazione Alterata
                                '''                      End Select
                                '''
                                '''                      'Evito la Division By zero
                                '''                      If mStoricoCarico = 0 Then
                                '''                         Prezzo_Unitario = 0
                                '''
                                '''                      Else
                                '''
                                '''                        ' Aggiornamento dello storico dei carichi del prodotto e determinazione della media ponderata
                                '''                        Prezzo_Unitario = Format(CDbl(CDbl(RsConsistenzeEno("Prezzo_Unitario")) * CDbl(RsConsistenzeEno("Sconto")) - CDbl(Prezzo_Unitario * Qta)) / (mStoricoCarico), "##,###,##0.00")
                                '''
                                '''                      End If

                            End If



                        Case "7350", "4200" 'Scarico, Conferimento a Diversi


                            If Operazione = "1" Then

                                'Inserimento di uno nuovo scarico:

                                '===========================================================================
                                'Gestione Consistenza Eno
                                If Jolly_Int = 0 Then
                                    'Il movimento ricade sulla gestione Consistenza Eno
                                    mQta = CDbl(dtConsistenzeEno.Rows(0).Item("Dett_Qta")) - Qta 'Quantità Attuale
                                Else
                                    'Il movimento non ricade sulla gestione Consistenza Eno
                                    mQta = CDbl(dtConsistenzeEno.Rows(0).Item("Dett_Qta")) 'Quantità invariata
                                End If

                                'Nota: uno scarico non ha conseguenze sul calcolo del valore ponderato
                                'mStoricoCarico = CDbl(RsConsistenzeEno("Sconto")) 'Invariato
                                'Prezzo_Unitario = CDbl(RsConsistenzeEno("Prezzo_Unitario"))

                                mStoricoCarico = CDbl(dtConsistenzeEno.Rows(0).Item("Sconto")) 'Invariato
                                Prezzo_Unitario = CDbl(dtConsistenzeEno.Rows(0).Item("Prezzo_Unitario"))



                            ElseIf Operazione = "3" Then

                                'Cancellazione di uno scarico

                                '===========================================================================
                                'Gestione Consistenza Eno
                                If Jolly_Int = 0 Then
                                    'Il movimento ricade sulla gestione Consistenza Eno
                                    mQta = CDbl(dtConsistenzeEno.Rows(0).Item("Dett_Qta")) + Qta 'Quantità Attuale
                                Else
                                    'Il movimento non ricade sulla gestione Consistenza Eno
                                    mQta = CDbl(dtConsistenzeEno.Rows(0).Item("Dett_Qta")) 'Quantità invariata
                                End If

                                mStoricoCarico = CDbl(dtConsistenzeEno.Rows(0).Item("Sconto")) 'Invariato
                                Prezzo_Unitario = CDbl(dtConsistenzeEno.Rows(0).Item("Prezzo_Unitario"))


                                ''                     '===========================================================================
                                ''                     'Gestione Valorizzazione Economica
                                ''                     Select Case Lav_Cod
                                ''
                                ''                        Case 1025, 1050, 1031, 1052: 'Bolle + Conferimenti
                                ''                           'Prezzo_Unitario Invariato
                                ''                            mStoricoCarico = CDbl(RsConsistenzeEno("Sconto")) 'Invariato
                                ''                            Prezzo_Unitario = CDbl(RsConsistenzeEno("Prezzo_Unitario"))
                                ''                        Case Else
                                ''
                                ''                            'E' stato cancellato un movimento che aveva conseguenze economiche
                                ''                            'Calcolo della nuova media ponderata
                                ''
                                ''                            mValorizzazione = CDbl(RsConsistenzeEno("Sconto")) * CDbl(RsConsistenzeEno("Prezzo_Unitario"))
                                ''                            mValorizzazione_Delete = Qta * Prezzo_Unitario
                                ''
                                ''                            mStoricoCarico = (mValorizzazione - mValorizzazione_Delete)
                                ''                            If (CDbl(RsConsistenzeEno("Sconto")) - Qta) > 0 And mStoricoCarico > 0 Then
                                ''                               'Evito il Division by Zero
                                ''                               Prezzo_Unitario = mStoricoCarico / (CDbl(RsConsistenzeEno("Sconto")) - Qta)
                                ''                            Else
                                ''                               Prezzo_Unitario = 0
                                ''                            End If
                                ''
                                ''                      End Select




                            End If

                    End Select

                    '==========================================================================

                    'Modifica della Consistenza Eno del prodotto
                    objMovimentiDettagliW.Modifica(CStr(Piva),
                                                    CInt(Sa_Cod),
                                                    Cod_Agenda,
                                                    Cod_Movimento,
                                                    Agro_SQL_Load(dtConsistenzeEno.Rows(0).Item("Id_Mov_Det")),
                                                    CInt(Elem_Cod), CInt(Pro_Cod), CInt(Mat_Cod),
                                                    "Rilevamento Consistenza Enologica del " & Format(CDate(Now), "dd/MM/yyyy"),
                                                    CDbl(mQta), CInt(Udm_Cod), 0, 0,
                                                    CDbl(mStoricoCarico),
                                                    CDbl(Prezzo_Unitario),
                                                    CDbl(Prezzo_Unitario),
                                                    CInt(Cal_Cod), 0,
                                                    CInt(Cod_Progetto), CInt(Fase_Cod),
                                                    "", 0, CDate(Now),
                                                    0, 0, 0, 0, 0,
                                                    1, 1, CStr(Lotto),
                                                    CInt(Udm_Cod_Extra), CDbl(Qta_Extra),
                                                    CDbl(Qta_Extra_Totale), 0, 0, 0, 0, 0,
                                                    0, "",
                                                    0, 0, 0,
                                                    Validita_Inizio,
                                                    Validita_Fine,
                                                    "",
                                                    objParametri)


            End Select

            objMovimentiDettagliW = Nothing
            dtConsistenzeEno.Dispose()
            dtConsistenzeEno = Nothing


            '=====================================================================================

            '=====================================================================================
            'Verifica dell'esistenza del riferimento alla vasca del prodotto
            '------------------------------------------------------------------------------------

            objMovDestinazioni = New AgronicaCoreContabDAL.Mov_Destinazioni_W

            'Leggo le Consistenze Eno della risorsa nella vasca

            dtConsistenzeEno = objMovimentiDettagliR.LeggiConsistenzeEnologiche(
                                                CStr(Piva),
                                                CInt(Sa_Cod),
                                                CInt(Elem_Cod),
                                                CInt(Pro_Cod),
                                                CInt(Mat_Cod),
                                                CInt(Udm_Cod),
                                                CInt(Id_Destinazione),
                                                CInt(Cal_Cod),
                                                CInt(Cod_Progetto),
                                                CInt(Fase_Cod),
                                                CStr(Lotto),
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "",
                                                "",
                                                objParametri)

            'Select Case RsConsistenzeEno.State
            Select Case dtConsistenzeEno.Rows.Count


                Case 0 'SALVA ----------------------------------------------------------------

                    'Salvo la destinazione
                    dummy = objMovDestinazioni.Scrivi(CStr(Piva),
                                                    CInt(Sa_Cod),
                                                    CInt(Cod_Agenda),
                                                    CInt(Cod_Movimento),
                                                    CInt(Cod_Movimento_Dettaglio),
                                                    0,
                                                    CInt(Id_Destinazione),
                                                    13,
                                                    CDbl(Qta),
                                                    0,
                                                    0,
                                                    0,
                                                    "",
                                                    0,
                                                    Validita_Inizio,
                                                    Validita_Fine,
                                                    objParametri)


                Case Else 'MODIFICA -----------------------------------------------------------

                    '================================================================================
                    'Aggiornamento delle Quantità presenti nel Magazzino.
                    '--------------------------------------------------------------------------------
                    If ((Cau_Mov = "7300" Or Cau_Mov = "4100") And Operazione = "1") Or
                       ((Cau_Mov = "7350" Or Cau_Mov = "4200") And Operazione = "3") Then


                        'Inserimento carico o cancellazione scarico

                        '===========================================================================
                        'Gestione Consistenza Eno
                        If Jolly_Int = 0 Then
                            'Il movimento ricade sulla gestione Consistenza Eno
                            mQta = CDbl(dtConsistenzeEno.Rows(0).Item("Qta")) + Qta 'Quantità Attuale
                        Else
                            'Il movimento non ricade sulla gestione Consistenza Eno
                            mQta = CDbl(dtConsistenzeEno.Rows(0).Item("Qta")) 'Quantità invariata
                        End If


                    ElseIf ((Cau_Mov = "7300" Or Cau_Mov = "4100") And Operazione = "3") Or
                           ((Cau_Mov = "7350" Or Cau_Mov = "4200") And Operazione = "1") Then


                        'Inserimento scarico o cancellazione carico

                        '===========================================================================
                        'Gestione Consistenza Eno
                        If Jolly_Int = 0 Then
                            'Il movimento ricade sulla gestione Consistenza Eno
                            mQta = CDbl(dtConsistenzeEno.Rows(0).Item("Qta")) - Qta 'Quantità Attuale
                        Else
                            'Il movimento non ricade sulla gestione Consistenza Eno
                            mQta = CDbl(dtConsistenzeEno.Rows(0).Item("Qta")) 'Quantità invariata
                        End If


                    End If

                    '==========================================================================

                    objMovDestinazioni.Modifica(CStr(Piva),
                                                CInt(Sa_Cod),
                                                CInt(Cod_Agenda),
                                                CInt(Cod_Movimento),
                                                CInt(dtConsistenzeEno.Rows(0).Item("Id_Mov_Det")),
                                                0,
                                                CInt(Id_Destinazione),
                                                13,
                                                CDbl(mQta),
                                                0,
                                                "",
                                                0,
                                                Validita_Inizio,
                                                Validita_Fine,
                                                "",
                                                objParametri)

            End Select


            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            dtConsistenzeEno.Dispose()
            dtConsistenzeEno = Nothing
            objMovDestinazioni = Nothing

            '------------------------------

            'Restituisco un valore Dummy
            xRisp = True

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            If flagTransazioneLocale = True Then
                objParametri.objTransazione.Commit()
            End If

            '----------------------------------------------------------------------------

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                objParametri.objTransazione.Rollback()
                objParametri.objTransazione = Nothing
            End If

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            If (flagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
                objParametri.objConnessione.Close()
            End If

        End Try

        Return xRisp


    End Function



    '============================================================================
    Public Function Consistenze_Scrivi(ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Elem_Cod As Integer,
                                       ByVal Pro_Cod As Integer,
                                       ByVal Mat_Cod As Integer,
                                       ByVal Udm_Cod As Integer,
                                       ByVal Id_Destinazione As Integer,
                                       ByVal Operazione As String,
                                       ByVal Cau_Mov As String,
                                       ByVal Cal_Cod As Integer,
                                       ByVal Cod_Progetto As Integer,
                                       ByVal Fase_Cod As Integer,
                                       ByVal Lotto As String,
                                       ByVal Qta As Decimal,
                                       ByVal Prezzo_Unitario As Decimal,
                                       ByVal Udm_Cod_Extra As Integer,
                                       ByVal Qta_Extra As Decimal,
                                       ByVal Qta_Extra_Totale As Decimal,
                                       ByVal Variazione As Decimal,
                                       ByVal Lav_Cod As Integer,
                                       ByVal Jolly_Int As Integer,
                                       ByVal Data_Movimento As Date,
                                       ByVal BaseCode As Integer,
                                       ByVal TopCode As Integer,
                                       ByVal Validita_Inizio As Date,
                                       ByVal Validita_Fine As Date,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As Boolean

        '----------------------------------------------------------------------

        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.Giacenze_W.Consistenze_Scrivi()"

        'Lav_Cod = -2
        'Tipo_Destinazione = 15 (Stalla)

        '----------------------------------------------------------------------

        Dim objMovimentiDettagliR As AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim objMovimentiDettagliW As AgronicaCoreContabDAL.Movimenti_Dettagli_W
        Dim objMovimenti As AgronicaCoreContabDAL.Movimenti_W
        Dim objMovDestinazioni As AgronicaCoreContabDAL.Mov_Destinazioni_W
        Dim objAgenda As AgronicaCoreContabDAL.Agenda_W
        Dim objSequenze As Agro_Sequenze
        Dim objAgroZooW As AgronicaCoreZooDAL.Zoo_Animali_W

        Dim dtConsistenze As DataTable

        Dim dummy As Boolean

        Dim Cod_Agenda As Integer
        Dim Cod_Movimento As Integer
        Dim Cod_Movimento_Dettaglio As Integer

        Dim mQta As Decimal
        Dim mStoricoCarico As Decimal

        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------


        Try

            '-------------------------------------------------------------------------------



            '===============================================================================
            'Verifica dell'esistenza dell'Id_Agenda relativo le consistenze di stalla
            '-------------------------------------------------------------------------------

            objMovimentiDettagliR = New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            objAgenda = New AgronicaCoreContabDAL.Agenda_W
            objMovimenti = New AgronicaCoreContabDAL.Movimenti_W


            'Lettura dell'Id_Agenda delle Consistenze
            dtConsistenze = objMovimentiDettagliR.LeggiConsistenze(CStr(Piva),
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    "",
                                                                    -1,
                                                                    -1,
                                                                    -1,
                                                                    -1,
                                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                    "",
                                                                    "",
                                                                    objParametri)


            'Select Case RsConsistenze.State
            Select Case dtConsistenze.Rows.Count

                Case 0 ' SALVA --------------------------------------------------------

                    'Creazione dell'Id_Agenda e del movimento relativo le consistenze

                    objSequenze = New Agro_Sequenze

                    'Richiedo un nuovo codice AGENDA
                    Cod_Agenda = objSequenze.NuovoId_Tabella("Agenda",
                                                            CInt(BaseCode),
                                                            CInt(TopCode),
                                                            objParametri)

                    dummy = objAgenda.Scrivi(CStr(Piva),
                                            0,
                                            CInt(Cod_Agenda),
                                            -2,
                                            0,
                                            0,
                                            0,
                                            "Gestione Consistenze di Stalla",
                                            0,
                                            0,
                                            "",
                                            AGRODATAINIZIO,
                                            Validita_Inizio,
                                            Validita_Fine,
                                            objParametri)


                    'Richiedo un nuovo codice MOVIMENTO
                    Cod_Movimento = objSequenze.NuovoId_Tabella("Movimenti",
                                                                CInt(BaseCode),
                                                                CInt(TopCode),
                                                                objParametri)

                    dummy = objMovimenti.Scrivi(CStr(Piva),
                                                0,
                                                CInt(Cod_Agenda),
                                                CInt(Cod_Movimento),
                                                0,
                                                "ANIMALI",
                                                "Rilevamento Consistenze",
                                                CDate(Date.Now),
                                                CDate(Date.Now),
                                                CDate(Date.Now),
                                                0, 0,
                                                0, 0, 0, 0, 0, 0, "", "", 0,
                                                AGRODATAINIZIO,
                                                0, 0, "", 0,
                                                AGRODATAINIZIO,
                                                "", "", "", 0, 0, 0, 0, "",
                                                0, 0, AGRODATAINIZIO, 0, 0,
                                                Validita_Inizio,
                                                Validita_Fine,
                                                0,
                                                objParametri)

                    objSequenze = Nothing


                Case Else ' LETTURA ---------------------------------------------------------

                    'Id_Agenda e id_Movimento Individuato

                    '=======================================================================
                    'Impostazione Parametri
                    '-----------------------------------------------------------------------
                    Cod_Agenda = dtConsistenze.Rows(0).Item("Id_Agenda")
                    Cod_Movimento = dtConsistenze.Rows(0).Item("Id_Mov")
                    '=======================================================================


            End Select

            objAgenda = Nothing
            objMovimenti = Nothing
            dtConsistenze.Dispose()
            dtConsistenze = Nothing

            '=====================================================================================


            '=====================================================================================
            'Verifica dell'esistenza dell'Id_Movimento_Dettaglio relativo le consistenze del'animale
            '-------------------------------------------------------------------------------------

            objMovimentiDettagliW = New AgronicaCoreContabDAL.Movimenti_Dettagli_W

            dtConsistenze = objMovimentiDettagliR.LeggiConsistenze(CStr(Piva),
                                                                    CStr(Sa_Cod),
                                                                    CInt(Elem_Cod),
                                                                    CInt(Pro_Cod),
                                                                    CInt(Mat_Cod),
                                                                    CInt(Udm_Cod),
                                                                    0,
                                                                    CInt(Cal_Cod),
                                                                    CInt(Cod_Progetto),
                                                                    CInt(Fase_Cod),
                                                                    CStr(Lotto),
                                                                    -1,
                                                                    -1,
                                                                    -1,
                                                                    -1,
                                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                    "",
                                                                    "",
                                                                    objParametri)


            Select Case dtConsistenze.Rows.Count


                Case 0 ' SALVA ---------------------------------------------------------------


                    'Creazione dell'Cod_Movimento_Dettaglio relativo la consistenza del'animale

                    'Richiedo un nuovo codice movimento_dettaglio
                    objSequenze = New Agro_Sequenze

                    Cod_Movimento_Dettaglio = objSequenze.NuovoId_Tabella("Movimenti_Dettagli",
                                                                          CInt(BaseCode),
                                                                          CInt(TopCode),
                                                                          objParametri)

                    objSequenze = Nothing

                    dummy = objMovimentiDettagliW.Scrivi(CStr(Piva),
                                                        CInt(Sa_Cod),
                                                        CInt(Cod_Agenda),
                                                        CInt(Cod_Movimento),
                                                        CInt(Cod_Movimento_Dettaglio),
                                                        CInt(Elem_Cod),
                                                        CInt(Pro_Cod), CInt(Mat_Cod),
                                                        "Rilevamento Consistenza del " & Format(CDate(Now), "dd/MM/yyyy"),
                                                        CDbl(Qta), CInt(Udm_Cod),
                                                        0, 0, CDbl(Qta),
                                                        CDbl(Prezzo_Unitario),
                                                        CDbl(Prezzo_Unitario),
                                                        0,
                                                        CInt(Cal_Cod),
                                                        CInt(Cod_Progetto),
                                                        CInt(Fase_Cod),
                                                        "", 0, CDate(Now),
                                                        0, 0, 0, 0, 0, 0,
                                                        1, 1, CStr(Lotto),
                                                        CInt(Udm_Cod_Extra), CDbl(Qta_Extra),
                                                        CDbl(Qta_Extra_Totale), 0,
                                                        CDbl(Variazione), 0, 0, 0, 0,
                                                        0, "",
                                                        0, 0, 0,
                                                        "", "", "",
                                                        Validita_Inizio,
                                                        Validita_Fine,
                                                        objParametri)

                    '
                Case Else 'MODIFICA -------------------------------------------------------

                    '=======================================================================
                    'Impostazione Parametri
                    '-----------------------------------------------------------------------
                    Cod_Movimento_Dettaglio = dtConsistenze.Rows(0).Item("Id_Mov_Det")
                    '=======================================================================

                    '==========================================================================
                    'Aggiornamento delle Quantità e della Valorizzazione della Consistenza.

                    'Calcolo della Valorizzazione Media Ponderata della Consistenza.
                    '--------------------------------------------------------------------------
                    Select Case Cau_Mov

                        Case "7300" 'Carico Consistenza

                            If Operazione = "1" Then

                                'Inserimento di uno nuovo carico:

                                '===========================================================================
                                'Gestione Consistenze
                                If Jolly_Int = 0 Then
                                    'Il movimento ricade sulla gestione consistenze
                                    mQta = CDbl(dtConsistenze.Rows(0).Item("Dett_Qta")) + Qta 'Quantità Attuale
                                Else
                                    'Il movimento non ricade sulla gestione consistenze
                                    mQta = CDbl(dtConsistenze.Rows(0).Item("Dett_Qta")) 'Quantità invariata
                                End If

                                '===========================================================================
                                'Gestione Valorizzazione Economica
                                Select Case Lav_Cod
                                    Case LAVCOD_NASCITA_ANIMALI ' 3000 Nascita
                                        mStoricoCarico = CDbl(dtConsistenze.Rows(0).Item("Sconto")) 'Valorizzazione Invariata
                                    Case Else
                                        mStoricoCarico = CDbl(dtConsistenze.Rows(0).Item("Sconto")) + Qta 'Valorizzazione Alterata
                                End Select


                                'Evito la Division By zero
                                If mStoricoCarico = 0 Then
                                    Prezzo_Unitario = 0

                                Else

                                    ' Aggiornamento dello storico dei carichi del prodotto e determinazione della media ponderata
                                    Prezzo_Unitario = Format(CDbl(CDbl(Prezzo_Unitario * Qta) + CDbl(dtConsistenze.Rows(0).Item("Prezzo_Unitario") * dtConsistenze.Rows(0).Item("Sconto"))) / (mStoricoCarico), "##,###,##0.00")

                                End If
                                '===========================================================================


                            ElseIf Operazione = "3" Then

                                'Cancellazione di un carico consistenza

                                '===========================================================================
                                'Gestione Consistenze
                                If Jolly_Int = 0 Then
                                    'Il movimento ricade sulla gestione consistenze
                                    mQta = CDbl(dtConsistenze.Rows(0).Item("Dett_Qta")) - Qta 'Quantità Attuale
                                Else
                                    'Il movimento non ricade sulla gestione consistenze
                                    mQta = CDbl(dtConsistenze.Rows(0).Item("Dett_Qta")) 'Quantità invariata
                                End If

                            End If



                        Case "7350" 'Scarico Consistenza


                            If Operazione = "1" Then

                                'Inserimento di uno nuovo scarico:

                                '===========================================================================
                                'Gestione Consistenze
                                If Jolly_Int = 0 Then
                                    'Il movimento ricade sulla gestione consistenze
                                    mQta = CDbl(dtConsistenze.Rows(0).Item("Dett_Qta")) - Qta 'Quantità Attuale
                                Else
                                    'Il movimento non ricade sulla gestione consistenze
                                    mQta = CDbl(dtConsistenze.Rows(0).Item("Dett_Qta")) 'Quantità invariata
                                End If

                                'Nota: uno scarico non ha conseguenze sul calcolo del valore ponderato

                                mStoricoCarico = CDbl(dtConsistenze.Rows(0).Item("Sconto")) 'Invariato
                                Prezzo_Unitario = CDbl(dtConsistenze.Rows(0).Item("Prezzo_Unitario"))

                            ElseIf Operazione = "3" Then

                                'Cancellazione di uno scarico

                                '===========================================================================
                                'Gestione Consistenze
                                If Jolly_Int = 0 Then
                                    'Il movimento ricade sulla gestione consistenze
                                    mQta = CDbl(dtConsistenze.Rows(0).Item("Dett_Qta")) + Qta 'Quantità Attuale
                                Else
                                    'Il movimento non ricade sulla gestione consistenze
                                    mQta = CDbl(dtConsistenze.Rows(0).Item("Dett_Qta")) 'Quantità invariata
                                End If

                                mStoricoCarico = CDbl(dtConsistenze.Rows(0).Item("Sconto")) 'Invariato
                                Prezzo_Unitario = CDbl(dtConsistenze.Rows(0).Item("Prezzo_Unitario"))

                            End If

                    End Select

                    '==========================================================================

                    'Modifica della consistenza dell'animale
                    objMovimentiDettagliW.Modifica(CStr(Piva),
                                                    CInt(Sa_Cod),
                                                    Cod_Agenda,
                                                    Cod_Movimento,
                                                    Agro_SQL_Load(dtConsistenze.Rows(0).Item("Id_Mov_Det")),
                                                    CInt(Elem_Cod),
                                                    CInt(Pro_Cod),
                                                    CInt(Mat_Cod),
                                                    "Rilevamento Consistenze del " & Format(CDate(Now), "dd/MM/yyyy"),
                                                    CDbl(mQta), CInt(Udm_Cod), 0, 0,
                                                    CDbl(mStoricoCarico),
                                                    CDbl(Prezzo_Unitario),
                                                    CDbl(Prezzo_Unitario),
                                                    CInt(Cal_Cod), 0,
                                                    CInt(Cod_Progetto), CInt(Fase_Cod),
                                                    "", 0, CDate(Now),
                                                    0, 0, 0, 0, 0,
                                                    1, 1, CStr(Lotto),
                                                    CInt(Udm_Cod_Extra), CDbl(Qta_Extra),
                                                    CDbl(Qta_Extra_Totale), 0, 0, 0, 0, 0,
                                                    0, "",
                                                    0, 0, 0,
                                                    Validita_Inizio,
                                                    Validita_Fine,
                                                    "",
                                                    objParametri)


                    'Modifica Validità della Consistenza
                    objAgroZooW = New AgronicaCoreZooDAL.Zoo_Animali_W

                    Select Case mQta

                        Case Is > 0 'La Consistenza è Ancora Valida

                            objAgroZooW.Modifica_Validita_Fine(Piva,
                                                               Cod_Progetto,
                                                               AGRODATAFINE,
                                                               "",
                                                               objParametri)

                        Case Else 'La Consistenza è Nulla quindi Non Valida

                            objAgroZooW.Modifica_Validita_Fine(Piva,
                                                               Cod_Progetto,
                                                               Format(Data_Movimento, "dd/MM/yyyy"),
                                                               "",
                                                               objParametri)

                    End Select

                    objAgroZooW = Nothing

            End Select


            objMovimentiDettagliW = Nothing
            dtConsistenze.Dispose()
            dtConsistenze = Nothing

            '=====================================================================================

            '=====================================================================================
            'Verifica dell'esistenza del riferimento alla stalla del prodotto
            '------------------------------------------------------------------------------------

            objMovDestinazioni = New AgronicaCoreContabDAL.Mov_Destinazioni_W

            'Leggo la consistenza dell'animale nella stalla

            dtConsistenze = objMovimentiDettagliR.LeggiConsistenze(CStr(Piva),
                                                                    CInt(Sa_Cod),
                                                                    CInt(Elem_Cod),
                                                                    CInt(Pro_Cod),
                                                                    CInt(Mat_Cod),
                                                                    CInt(Udm_Cod),
                                                                    CInt(Id_Destinazione),
                                                                    CInt(Cal_Cod),
                                                                    CInt(Cod_Progetto),
                                                                    CInt(Fase_Cod),
                                                                    CStr(Lotto),
                                                                    -1,
                                                                    -1,
                                                                    -1,
                                                                    -1,
                                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                    "",
                                                                    "",
                                                                    objParametri)


            'Select Case RsConsistenze.State
            Select Case dtConsistenze.Rows.Count

                Case 0 'SALVA ----------------------------------------------------------------

                    'Salvo la destinazione
                    dummy = objMovDestinazioni.Scrivi(CStr(Piva),
                                                    CInt(Sa_Cod),
                                                    CInt(Cod_Agenda),
                                                    CInt(Cod_Movimento),
                                                    CInt(Cod_Movimento_Dettaglio),
                                                    0,
                                                    CInt(Id_Destinazione),
                                                    15,
                                                    CDbl(Qta),
                                                    0,
                                                    0,
                                                    0,
                                                    "",
                                                    0,
                                                    Validita_Inizio,
                                                    Validita_Fine,
                                                    objParametri)


                Case Else 'MODIFICA -----------------------------------------------------------

                    '================================================================================
                    'Aggiornamento delle Quantità presenti nella Stalla.
                    '--------------------------------------------------------------------------------
                    If ((Cau_Mov = "7300") And Operazione = "1") Or
                       ((Cau_Mov = "7350") And Operazione = "3") Then


                        'Inserimento carico o cancellazione scarico

                        '===========================================================================
                        'Gestione Consistenze
                        If Jolly_Int = 0 Then
                            'Il movimento ricade sulla gestione consistenza
                            mQta = CDbl(dtConsistenze.Rows(0).Item("Qta")) + Qta 'Quantità Attuale
                        Else
                            'Il movimento non ricade sulla gestione consistenza
                            mQta = CDbl(dtConsistenze.Rows(0).Item("Qta")) 'Quantità invariata
                        End If


                    ElseIf ((Cau_Mov = "7300") And Operazione = "3") Or
                           ((Cau_Mov = "7350") And Operazione = "1") Then


                        'Inserimento scarico o cancellazione carico

                        '===========================================================================
                        'Gestione Consistenza
                        If Jolly_Int = 0 Then
                            'Il movimento ricade sulla gestione consistenza
                            mQta = CDbl(dtConsistenze.Rows(0).Item("Qta")) - Qta 'Quantità Attuale
                        Else
                            'Il movimento non ricade sulla gestione consistenza
                            mQta = CDbl(dtConsistenze.Rows(0).Item("Qta")) 'Quantità invariata
                        End If


                    End If

                    '==========================================================================

                    objMovDestinazioni.Modifica(CStr(Piva),
                                                CInt(Sa_Cod),
                                                CInt(Cod_Agenda),
                                                CInt(Cod_Movimento),
                                                CInt(dtConsistenze.Rows(0).Item("Id_Mov_Det")),
                                                0,
                                                CInt(Id_Destinazione),
                                                15,
                                                CDbl(mQta),
                                                0,
                                                "",
                                                0,
                                                Validita_Inizio,
                                                Validita_Fine,
                                                "",
                                                objParametri)

            End Select


            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            dtConsistenze.Dispose()
            dtConsistenze = Nothing
            objMovDestinazioni = Nothing

            '------------------------------

            'Restituisco un valore Dummy
            xRisp = True

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            If flagTransazioneLocale = True Then
                objParametri.objTransazione.Commit()
            End If

            '----------------------------------------------------------------------------

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                objParametri.objTransazione.Rollback()
                objParametri.objTransazione = Nothing
            End If

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            If (flagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
                objParametri.objConnessione.Close()
            End If

        End Try

        Return xRisp


    End Function


    '##############################################################################################
    'le funzioni di lettura delle giacenze sono anche nella classe Giacenze di questo core
    'questa era la query dei componenti
    Private Function LeggiGiacenze_NONUSARE(ByVal PIVA As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Elem_Cod As Integer,
                                            ByVal Pro_Cod As Integer,
                                            ByVal Mat_Cod As Integer,
                                            ByVal Udm_Cod As Integer,
                                            ByVal Id_Destinazione As Integer,
                                            ByVal Cal_Cod As Integer,
                                            ByVal Cod_Progetto As Integer,
                                            ByVal Fase_Cod As Integer,
                                            ByVal Lotto As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.LeggiGiacenze()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            strSql.Length = 0

            'Query per il prelievo dei dati                 ' #### CLASSE ####

            strSql.Append(" SELECT Agenda.* , Movimenti.* , Movimenti_Dettagli.* , Mov_Destinazioni.* , Mov_Destinazioni.Qta as Dest_Qta , Movimenti_Dettagli.Qta as Dett_Qta  ")
            strSql.Append(" FROM  Agenda (NOLOCK) , Movimenti (NOLOCK) , Movimenti_Dettagli (NOLOCK) , Mov_Destinazioni (NOLOCK) ")
            strSql.Append(" WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.Append(" AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            strSql.Append(" AND   Agenda.Lav_Cod = -1    ")
            strSql.Append(" AND   Movimenti.Cau_Mov = 'GIACENZE'    ")
            strSql.Append(" AND   Mov_Destinazioni.Tipo_Destinazione = 20   ")

            'Join sulla Piva
            strSql.Append(" AND   Agenda.Piva = Movimenti.Piva ")
            strSql.Append(" AND   Agenda.Piva = Movimenti_Dettagli.Piva ")
            strSql.Append(" AND   Agenda.Piva = Mov_Destinazioni.Piva ")
            strSql.Append(" AND   Movimenti.Piva = Movimenti_Dettagli.Piva ")
            strSql.Append(" AND   Movimenti.Piva = Mov_Destinazioni.Piva ")
            strSql.Append(" AND   Movimenti_Dettagli.Piva = Mov_Destinazioni.Piva ")

            'Join sul Sa_Cod
            strSql.Append(" AND   Agenda.Sa_Cod = Movimenti.Sa_Cod ")
            strSql.Append(" AND   Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")

            'Join su Id_Agenda
            strSql.Append(" AND   Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.Append(" AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            strSql.Append(" AND   Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")

            'Join su Id_Mov
            strSql.Append(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            strSql.Append(" AND   Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")

            'Join su Id_Mov_Det
            strSql.Append(" AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

            If PIVA <> "" Then
                strSql.Append(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                strSql.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.Append(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.Append(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.Append(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                strSql.Append(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If Id_Destinazione <> 0 Then
                strSql.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
            End If

            If Cal_Cod <> 0 Then
                strSql.Append(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
            End If

            If Cod_Progetto <> 0 Then
                strSql.Append(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
            End If

            If Fase_Cod <> 0 Then
                strSql.Append(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            End If

            If Lotto <> "" Then
                strSql.Append(" AND Upper(Movimenti_Dettagli.Lotto) = '" & UCase(Agro_SQL_SaveText(Lotto)) & "'   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Movimenti_Dettagli.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Movimenti_Dettagli.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY Movimenti_Dettagli.Elem_Cod Asc ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '##############################################################################################
    '#############################################################################################
    'Routine per la pulizia (cancellazione) dei prodotti creati da operazioni rintracciabili
    'e poi cancellate. Tali prodotti sono di fatto 'ghost'.

    'Nota Importante: la routine di pulizia viene lanciata per singolo prodotto. Questa soluzione è necessaria
    'per evitare il time-out del componente in caso di un elevato numero di prodotti giacenti in magazzino.
    '============================================================================
    Public Function Giacenza_Pulizia(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Elem_Cod As Integer,
                                     ByVal Pro_Cod As Integer,
                                     ByVal Mat_Cod As Integer,
                                     ByVal Udm_Cod As Integer,
                                     ByVal Cal_Cod As Integer,
                                     ByVal Cod_Progetto As Integer,
                                     ByVal Fase_Cod As Integer,
                                     ByVal Lotto_Prodotto As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                     ) As String

        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.Giacenze_W.Giacenza_Pulizia()"
        Dim messaggioErrore As String = ""
        Dim flagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction
        Dim risultatoFunzione As String = String.Empty

        Dim chiave(,) As String
        Dim indice As Integer
        Dim bDuplicato As Boolean

        ' Const MetodoNome = "Giacenza_Pulizia:"

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                flagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                xTransazione = Nothing
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
                xTransazione = objParametri.objTransazione
            End If

            '------------------------------
            Dim objMovimentiDettagliR As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            Dim objMovimentiDettagliW As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
            Dim objMovDestinazioni As New AgronicaCoreContabDAL.Mov_Destinazioni_W
            Dim objMovDestinazioniR As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            Dim objMpCampionature As New AgronicaCoreContabDAL.Materie_Prime_Campion_W

            Dim dtGiacenze As DataTable
            Dim dtCaricoScarico As DataTable
            Dim dtdummy As DataTable

            ''Lettura dell'Id_Agenda delle giacenze
            dtGiacenze = LeggiGiacenze_NONUSARE(CStr(Piva),
                                                CLng(Sa_Cod),
                                                CLng(Elem_Cod),
                                                CLng(Pro_Cod),
                                                CLng(Mat_Cod),
                                                CLng(Udm_Cod),
                                                0,
                                                CLng(Cal_Cod),
                                                CLng(Cod_Progetto),
                                                CLng(Fase_Cod),
                                                CStr(Lotto_Prodotto),
                                                "",
                                                "",
                                                objParametri)

            ''DtGiacenze = New AgronicaCoreContabDAL.Giacenze_R().SchedaGiacenzeMagazzino( _
            ''                         AGRODATAFINE, _
            ''                        CStr(Piva), _
            ''                          CLng(Sa_Cod), _
            ''                          0, _
            ''                          CLng(Elem_Cod), _
            ''                        CLng(Pro_Cod), _
            ''                        CLng(Mat_Cod), _
            ''                        CLng(Cal_Cod), _
            ''                        CLng(Cod_Progetto), _
            ''                        CLng(Fase_Cod), _
            ''                          CLng(Udm_Cod), _
            ''                            CStr(Lotto_Prodotto), False, _
            ''                           "", "", "", "", "", "", "", "", "", "", "", "", objParametri)



            Giacenza_Pulizia = "0" 'Inizializzazione

            Select Case dtGiacenze.Rows.Count

                Case 0 ' SALVA --------------------------------------------------------

                    'Non esiste il record delle giacenze

                    'Do Nothing


                Case Else ' LETTURA ---------------------------------------------------------

                    'Id_Agenda e id_Movimento Individuato

                    '=======================================================================
                    'Imposto il Filtro sugli Elem_Cod
                    '-----------------------------------------------------------------------

                    '25/09/07 ore 11:08 'Annullamento Filtro ordinato dalla Maga ed eseguito dal SuxVisor.

                    'Inizialmente il filtro serviva solo per le categorie di prodotti rintracciabili perchè
                    'erano queste che in caso di raccolte/trasformazioni cancellate creavano dati ghost.
                    'Successivamente si è verificato il caso in cui l'utente inseriva erroneamente una giacenza con
                    'una udm diversa da quella che realmente gli serviva; così facendo il gestore delle giacenze
                    'proponeva sempre l'udm inutile con qta = 0 dopo che l'utente aveva cancellato il carico errato.
                    'Di fatto dal punto di vista prettamente funzionale tale giacenza rappresenta un ghost.


                    'RsGiacenze.Filter = "Elem_Cod = 201 Or " & _
                    '                 "Elem_Cod = 210 Or " & _
                    '                 "Elem_Cod = 301 Or " & _
                    '                 "Elem_Cod = 310 Or " & _
                    '                 "Elem_Cod = 10  "

                    'Nota: Correzione Baco Dettagli Giacenza Duplicati.
                    'A causa dell'update del Lotto = "" Where Lotto = "-1" (vecchissima gestione raccolta)
                    'in caso di modifiche sulla raccolta i record dei dettagli venivano duplicati.
                    'Utilizzo pertanto una struttura di appoggio (chiave) in cui memorizzo la chiave
                    'del prodotto giacente ed in caso di solo id_mov_det diverso procederò con la sua
                    'cancellazione.

                    ReDim chiave(14, 0)

                    'Do While Not RsGiacenze.EOF
                    Dim iGia As Integer
                    For iGia = 0 To dtGiacenze.Rows.Count - 1

                        bDuplicato = False
                        For indice = 0 To UBound(chiave, 2) - 1

                            If chiave(0, indice) = dtGiacenze.Rows(iGia).Item("Piva") And
                               chiave(1, indice) = dtGiacenze.Rows(iGia).Item("Sa_Cod") And
                               chiave(2, indice) = dtGiacenze.Rows(iGia).Item("Elem_Cod") And
                               chiave(3, indice) = dtGiacenze.Rows(iGia).Item("Pro_Cod") And
                               chiave(4, indice) = dtGiacenze.Rows(iGia).Item("Mat_Cod") And
                               chiave(5, indice) = dtGiacenze.Rows(iGia).Item("Udm_Cod") And
                               chiave(6, indice) = dtGiacenze.Rows(iGia).Item("Cal_Cod") And
                               chiave(7, indice) = dtGiacenze.Rows(iGia).Item("Cod_Progetto") And
                               chiave(8, indice) = dtGiacenze.Rows(iGia).Item("Fase_Cod") And
                               chiave(9, indice) = dtGiacenze.Rows(iGia).Item("Lotto") And
                               chiave(10, indice) = dtGiacenze.Rows(iGia).Item("Id_Destinazione") And
                               chiave(11, indice) = dtGiacenze.Rows(iGia).Item("Id_Agenda") And
                               chiave(12, indice) = dtGiacenze.Rows(iGia).Item("Id_Mov") And
                               chiave(13, indice) <> dtGiacenze.Rows(iGia).Item("Id_Mov_Det") Then

                                bDuplicato = True

                                Exit For
                            End If

                        Next indice


                        Select Case bDuplicato

                            Case True

                                '##########################################################################################
                                '######################   Cancellazione della Giacenza   ##################################
                                '##########################################################################################

                                'Cancellazione Destinazione Giacenza

                                objMovDestinazioni.Cancella(dtGiacenze.Rows(iGia).Item("Piva"),
                                                            0,
                                                            CLng(dtGiacenze.Rows(iGia).Item("Id_Agenda")),
                                                            CLng(dtGiacenze.Rows(iGia).Item("Id_Mov")),
                                                            CLng(dtGiacenze.Rows(iGia).Item("Id_Mov_Det")),
                                                            0, 0,
                                                            "",
                                                            objParametri)


                                'Cancellazione Dettaglio Giacenza
                                objMovimentiDettagliW.Cancella(dtGiacenze.Rows(iGia).Item("Piva"),
                                                               0,
                                                               CLng(dtGiacenze.Rows(iGia).Item("Id_Agenda")),
                                                               CLng(dtGiacenze.Rows(iGia).Item("Id_Mov")),
                                                               CLng(dtGiacenze.Rows(iGia).Item("Id_Mov_Det")),
                                                               "",
                                                               objParametri)

                                Giacenza_Pulizia = "1" 'Avvenuta Cancellazione

                            Case False

                                'Nota: Correzione Baco set Lotto ='' where Lotto = -1 -->Cancellazione dei duplicati!!!
                                ReDim Preserve chiave(14, UBound(chiave, 2) + 1)

                                chiave(0, UBound(chiave, 2) - 1) = dtGiacenze.Rows(iGia).Item("Piva")
                                chiave(1, UBound(chiave, 2) - 1) = dtGiacenze.Rows(iGia).Item("Sa_Cod")
                                chiave(2, UBound(chiave, 2) - 1) = dtGiacenze.Rows(iGia).Item("Elem_Cod")
                                chiave(3, UBound(chiave, 2) - 1) = dtGiacenze.Rows(iGia).Item("Pro_Cod")
                                chiave(4, UBound(chiave, 2) - 1) = dtGiacenze.Rows(iGia).Item("Mat_Cod")
                                chiave(5, UBound(chiave, 2) - 1) = dtGiacenze.Rows(iGia).Item("Udm_Cod")
                                chiave(6, UBound(chiave, 2) - 1) = dtGiacenze.Rows(iGia).Item("Cal_Cod")
                                chiave(7, UBound(chiave, 2) - 1) = dtGiacenze.Rows(iGia).Item("Cod_Progetto")
                                chiave(8, UBound(chiave, 2) - 1) = dtGiacenze.Rows(iGia).Item("Fase_Cod")
                                chiave(9, UBound(chiave, 2) - 1) = dtGiacenze.Rows(iGia).Item("Lotto")
                                chiave(10, UBound(chiave, 2) - 1) = dtGiacenze.Rows(iGia).Item("Id_Destinazione")
                                chiave(11, UBound(chiave, 2) - 1) = dtGiacenze.Rows(iGia).Item("Id_Agenda")
                                chiave(12, UBound(chiave, 2) - 1) = dtGiacenze.Rows(iGia).Item("Id_Mov")
                                chiave(13, UBound(chiave, 2) - 1) = dtGiacenze.Rows(iGia).Item("Id_Mov_Det")

                                'Lettura dei Movimenti del Prodotto

                                'Carico/Scarico in/da Magazzino

                                dtCaricoScarico =
                                    objMovimentiDettagliR.LeggiCaricoScarico_New_Puntuale(dtGiacenze.Rows(iGia).Item("Piva"),
                                                                                          dtGiacenze.Rows(iGia).Item("Sa_Cod"),
                                                                                          0, 0, 0,
                                                                                          CLng(dtGiacenze.Rows(iGia).Item("Elem_Cod")),
                                                                                          CLng(dtGiacenze.Rows(iGia).Item("Pro_Cod")),
                                                                                          CLng(dtGiacenze.Rows(iGia).Item("Mat_Cod")),
                                                                                          CLng(dtGiacenze.Rows(iGia).Item("Udm_Cod")),
                                                                                          0,
                                                                                          0,
                                                                                          20,
                                                                                          "",
                                                                                          CLng(dtGiacenze.Rows(iGia).Item("Cod_Progetto")),
                                                                                          CLng(dtGiacenze.Rows(iGia).Item("Fase_Cod")),
                                                                                          0,
                                                                                          0,
                                                                                          CStr(dtGiacenze.Rows(iGia).Item("Lotto")),
                                                                                          CLng(dtGiacenze.Rows(iGia).Item("Cal_Cod")),
                                                                                          "",
                                                                                          AGRODATAINIZIO,
                                                                                          AGRODATAFINE,
                                                                                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                          "",
                                                                                          "",
                                                                                          objParametri)


                                If dtCaricoScarico.Rows.Count = 0 Then

                                    'Nessun Record Corrispondente

                                    '##########################################################################################
                                    '######################   Cancellazione della Giacenza   ##################################
                                    '##########################################################################################

                                    'Cancellazione Destinazione Giacenza
                                    objMovDestinazioni.Cancella(dtGiacenze.Rows(iGia).Item("Piva"),
                                                                0,
                                                                CLng(dtGiacenze.Rows(iGia).Item("Id_Agenda")),
                                                                CLng(dtGiacenze.Rows(iGia).Item("Id_Mov")),
                                                                CLng(dtGiacenze.Rows(iGia).Item("Id_Mov_Det")),
                                                                0, 0,
                                                                "",
                                                                objParametri)



                                    'Cancellazione Dettaglio Giacenza
                                    objMovimentiDettagliW.Cancella(dtGiacenze.Rows(iGia).Item("Piva"),
                                                                   0,
                                                                   CLng(dtGiacenze.Rows(iGia).Item("Id_Agenda")),
                                                                   CLng(dtGiacenze.Rows(iGia).Item("Id_Mov")),
                                                                   CLng(dtGiacenze.Rows(iGia).Item("Id_Mov_Det")),
                                                                   "",
                                                                   objParametri)

                                    'Cancellazione Campionatura
                                    If CLng(dtGiacenze.Rows(iGia).Item("Cal_Cod")) < 0 Then

                                        objMpCampionature.Cancella(CLng(dtGiacenze.Rows(iGia).Item("Cal_Cod")),
                                                                   "", 0, 0,
                                                                   True,
                                                                   "",
                                                                   objParametri)

                                    End If

                                    Giacenza_Pulizia = "1" 'Avvenuta Cancellazione


                                Else 'Giacenza Complessiva Presente


                                    'Controllo che il prodotto sia movimentato nel magazzino

                                    Dim drCaricoScarico() As DataRow
                                    drCaricoScarico = dtCaricoScarico.Select("Id_Destinazione = " & dtGiacenze.Rows(iGia).Item("Id_Destinazione"))

                                    If drCaricoScarico Is Nothing Or drCaricoScarico.Length = 0 Then
                                        'Il prodotto potrebbe essere movimentato da altri magazzini, ma non da questo.
                                        'Elimino il record delle giacenze in modo da consentirne eventualmente la cancellazione.


                                        'Cancellazione Destinazione Giacenza del Magazzino
                                        objMovDestinazioni.Cancella(dtGiacenze.Rows(iGia).Item("Piva"),
                                                                    0,
                                                                    CLng(dtGiacenze.Rows(iGia).Item("Id_Agenda")),
                                                                    CLng(dtGiacenze.Rows(iGia).Item("Id_Mov")),
                                                                    CLng(dtGiacenze.Rows(iGia).Item("Id_Mov_Det")),
                                                                    0,
                                                                    CLng(dtGiacenze.Rows(iGia).Item("Id_Destinazione")),
                                                                    "",
                                                                    objParametri)

                                        'Controllo che il dettaglio abbia almeno una corrispondente destinazione
                                        dtdummy = objMovDestinazioniR.Leggi(dtGiacenze.Rows(iGia).Item("Piva"),
                                                                            0,
                                                                            CLng(dtGiacenze.Rows(iGia).Item("Id_Agenda")),
                                                                            CLng(dtGiacenze.Rows(iGia).Item("Id_Mov")),
                                                                            CLng(dtGiacenze.Rows(iGia).Item("Id_Mov_Det")),
                                                                            0,
                                                                            0,
                                                                            0,
                                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                            "",
                                                                            "",
                                                                            objParametri)

                                        If dtdummy.Rows.Count = 0 Then

                                            'Cancellazione Dettaglio Giacenza
                                            objMovimentiDettagliW.Cancella(dtGiacenze.Rows(iGia).Item("Piva"),
                                                                           0,
                                                                           CLng(dtGiacenze.Rows(iGia).Item("Id_Agenda")),
                                                                           CLng(dtGiacenze.Rows(iGia).Item("Id_Mov")),
                                                                           CLng(dtGiacenze.Rows(iGia).Item("Id_Mov_Det")),
                                                                           "",
                                                                           objParametri)

                                        End If

                                        Giacenza_Pulizia = "1" 'Avvenuta Cancellazione

                                    End If

                                End If

                        End Select


                    Next iGia

            End Select


            '##################################################################################
            '############ CONTROLLO ESISTENZA MOVIMENTAZIONE GIACENZE  ########################
            '##################################################################################


            'Lettura dell'Id_Agenda delle giacenze
            dtGiacenze = LeggiGiacenze_NONUSARE(CStr(Piva),
                                                0,
                                                0,
                                                0,
                                                0,
                                                0,
                                                0,
                                                0,
                                                0,
                                                0,
                                                "",
                                                "",
                                                "",
                                                objParametri)


            If dtGiacenze.Rows.Count = 0 Then

                'Cancellazione dell'ID AGENDA delle giacenze
                Dim objAgendaLeggi As New AgronicaCoreContabBIZ.Agenda_R
                Dim objAgendaScrivi As New AgronicaCoreContabBIZ.Agenda_W
                Dim xmlAgenda As String
                Dim risp As Boolean

                xmlAgenda = objAgendaLeggi.Agenda_Leggi(Piva, 0, 0, -1, True, objParametri)
                objAgendaLeggi = Nothing

                If xmlAgenda <> "" Then

                    risp = objAgendaScrivi.Agenda_Scrivi(xmlAgenda,
                                                        Nothing,
                                                        0,
                                                        5,
                                                        0,
                                                        "",
                                                        objParametri)

                End If

                objAgendaScrivi = Nothing

                Giacenza_Pulizia = "1" 'Avvenuta Cancellazione

                '--------------------------------------------------------------------------------------------------

            End If

            objMovimentiDettagliR = Nothing
            objMovimentiDettagliW = Nothing
            objMovDestinazioniR = Nothing
            objMovDestinazioni = Nothing
            objMpCampionature = Nothing
            dtGiacenze = Nothing
            dtCaricoScarico = Nothing


            '------------------------------

        Catch ex As Exception

            risultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            'Restituisco un valore Dummy
            Giacenza_Pulizia = "-1"

        Finally

            If flagConnessioneLocale = True Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    xConnessione.Close()
                End If
            End If

        End Try


    End Function


End Class

Public Class Giacenze_R
    Public Function LeggiProdottiDaTrattare_QdC(piva As String,
                                                sa_cod As Integer,
                                                specie As Specie,
                                                validitaFine As DateTime,
                                                tipoAttivita As Attivita.Tipo_Attivita,
                                                elem_cod As Integer,
                                                objParametri_Super_Server As AgronicaCoreParametri,
                                                objParametri_Server As AgronicaCoreParametri,
                                                objParametri_Utenti As AgronicaCoreParametri
                                                ) As List(Of MovimentoDiMagazzino)

        Dim movimentiMagazzino As New List(Of MovimentoDiMagazzino)

        Dim Veg_Cod = specie.codice

        Dim DtRisultati As DataTable

        Dim DTProdottiMagazzino = New DataTable
        DTProdottiMagazzino.Columns.Add(New DataColumn("Testo", GetType(String)))
        DTProdottiMagazzino.Columns.Add(New DataColumn("MovimentoMagazzino", GetType(MovimentoDiMagazzino)))
        DTProdottiMagazzino.Columns.Add(New DataColumn("Mat_Des", GetType(String)))

        Dim Filtro = " AND (Fabbricati.Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(validitaFine) & " AND Fabbricati.Validita_Fine >= " & UtilityProvider.Agro_SQL_SaveDate(validitaFine) & ")"
        If sa_cod <> 0 Then
            Filtro &= " AND Fabbricati.Sa_Cod = " & UtilityProvider.Agro_SQL_SaveNum(sa_cod) & " "
        End If


        Dim FiltroMateriePrime As String = ""
        Dim FiltroTrasformatiVegetali As String = ""
        Select Case elem_cod
            Case SEMENTI
                FiltroMateriePrime = " AND Materie_Prime.Veg_Cod = " & UtilityProvider.Agro_SQL_SaveNum(specie.codice)
            Case TRASFORMATI_VEGETALI
                FiltroTrasformatiVegetali = " AND Materie_Prime.Veg_Cod = " & UtilityProvider.Agro_SQL_SaveNum(specie.codice)
        End Select

        'DT: sa_cod ininfluente, le materie prime non sono usate per centro aziendale
        Dim objG As New AgronicaCoreStampeDAL.Magazzino
        DtRisultati = objG.SchedaGiacenzeMagazzino(validitaFine,
                                                   piva, 0, 0,
                                                   elem_cod,
                                                   0,
                                                   0,
                                                   0, 0, 0, 0,
                                                   LOTTO_NONDEFINITO,
                                                   Flag_QtaNoZero:=True, 'DT: ex escludiGiacenzeZero
                                                   Filtro,
                                                   "",
                                                   "", "",
                                                   "", "",
                                                   "", FiltroMateriePrime,
                                                   "", "",
                                                   FiltroTrasformatiVegetali, "",
                                                   "",
                                                   objParametri_Server, objParametri_Utenti, "",
                                                   Flag_QtaMaggioreZero:=True)

        If tipoAttivita <> Attivita.Tipo_Attivita.QuadernoDiCampagna Then
            'Nelle ricette non gestiamo il Progetto_Cod (la tabella non lo ammette, raggruppo le altre componenti della chiave (Mat_Cod e Lotto) e sommo le giacenze
            Dim dicProdotti As New Dictionary(Of String, Decimal)
            For Each row In DtRisultati.Rows
                Dim chiave As String = CStr(row.item("Mat_Cod")) & "_" & CStr(row.item("Lotto"))
                If dicProdotti.ContainsKey(chiave) Then
                    dicProdotti(chiave) += row.item("Giacenza")
                Else
                    dicProdotti.Add(chiave, row.item("Giacenza"))
                End If
            Next

            DtRisultati = DtRisultati.AsEnumerable().
                          GroupBy(Function(x) New With {Key .Mat_Cod = x.Item("Mat_Cod"), Key .Lotto = x.Item("Lotto")}).
                          Select(Function(g) g.First()).
                          CopyToDataTable()

            For Each row In DtRisultati.Rows
                Dim chiave As String = CStr(row.item("Mat_Cod")) & "_" & CStr(row.item("Lotto"))
                row.item("Giacenza") = dicProdotti(chiave) 'Aggiorno la giacenza con il valore sommato
                row.item("Cod_Progetto") = 0 'Azzero il codice progetto
            Next
        End If

        If Not IsNothing(DtRisultati) AndAlso DtRisultati.Rows.Count > 0 Then
            Dim objUdm As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
            Dim objMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

            Dim listMatCod = DtRisultati.AsEnumerable().Select(Function(row) row.Field(Of Integer)("Mat_Cod")).Distinct().ToList()
            Dim DTFinalita = objMateriePrime.LeggiFinalitaMateriePrime(piva, TRASFORMATI_VEGETALI, 0, Veg_Cod, " Mat_Cod IN (" & String.Join(",", listMatCod) & ")", "", objParametri_Server)

            For i = 0 To DtRisultati.Rows.Count - 1

                Dim Mat_Cod = DtRisultati.Rows(i).Item("Mat_Cod")
                Dim Mat_Des = DtRisultati.Rows(i).Item("Descrizione_Prodotto")
                Dim _Elem_Cod = DtRisultati.Rows(i).Item("Elem_Cod")
                Dim udm_carico = DtRisultati.Rows(i).Item("Udm_Cod")

                Select Case udm_carico
                    Case enum_UnitaMisura.Tonnellate, enum_UnitaMisura.Quintali, enum_UnitaMisura.KG, enum_UnitaMisura.Grammi, enum_UnitaMisura.Milligrammi
                    Case Else
                        'Se il prodotto è stato caricato usando un'unità di misura diversa da quelle previste, non mostro il prodotto in griglia
                        Continue For
                End Select

                Dim qtaToQuintali As Decimal = objUdm.Converti(udm_carico, DtRisultati.Rows(i).Item("Giacenza"), enum_UnitaMisura.Quintali)

                Dim Grfi_Cod As Integer = 0
                Dim Grfi_Des As String = ""
                Dim Finalita = DTFinalita.Select("Mat_Cod = " & Mat_Cod).FirstOrDefault()
                If Finalita IsNot Nothing Then
                    Grfi_Cod = Finalita.Item("Grfi_Cod")
                    Grfi_Des = Finalita.Item("Grfi_Des")
                End If

                Dim movimentoMagazzino = New MovimentoDiMagazzino With {
                    .Prodotto = New risorse.Prodotto(Mat_Cod, _Elem_Cod) With {
                        .descrizione = Mat_Des,
                        .varieta = New utilizzi.Varieta(DtRisultati.Rows(i).Item("Cul_cod")) With {
                            .specie = New utilizzi.Specie(DtRisultati.Rows(i).Item("Veg_cod")) With {
                                .descrizione = DtRisultati.Rows(i).Item("Veg_Des")
                            }
                        },
                        .specie = New utilizzi.Specie(DtRisultati.Rows(i).Item("Veg_cod")) With {
                                .descrizione = DtRisultati.Rows(i).Item("Veg_Des")
                        },
                        .regolamento = New Regolamenti(DtRisultati.Rows(i).Item("Regolamento")),
                        .codice_alfanumerico = DtRisultati.Rows(i).Item("Cod_Articolo"),
                        .finalita = New GruppoFinalita(Grfi_Cod, Grfi_Des)
                    },
                    .codice_progetto = DtRisultati.Rows(i).Item("Cod_Progetto"),
                    .Qta = Math.Round(qtaToQuintali, 4),
                    .UdM = New AgronicaCoreModelsSTD.metaschema.UnitaDiMisura With {
                        .codice = enum_UnitaMisura.Quintali
                    },
                    .Lotto = DtRisultati.Rows(i).Item("lotto"),
                    .Magazzino = New AgronicaCoreModelsSTD.anagrafiche.Fabbricato With {
                    .primaryKey = New anagrafiche.Fabbricato.PK With {
                        .centroAziendalePK = New anagrafiche.CentroAziendale.PK With {
                            .partitaIva = DtRisultati.Rows(i).Item("Piva"),
                            .codice = DtRisultati.Rows(i).Item("Sa_Cod")
                        },
                        .codice = DtRisultati.Rows(i).Item("Id_Destinazione")
                     },
                    .descrizione = CStr(DtRisultati.Rows(i).Item("Fabbricato_Des")),
                    .descrizione_centro = CStr(DtRisultati.Rows(i).Item("Sa_Nome"))
                    }
                }

                Dim Dr = DTProdottiMagazzino.NewRow
                Dr.Item("Mat_Des") = Mat_Des
                Dr.Item("MovimentoMagazzino") = movimentoMagazzino

                DTProdottiMagazzino.Rows.Add(Dr)
            Next
        End If

        If DTProdottiMagazzino IsNot Nothing Then

            'uso il dataview per ordinare 
            Dim Dv As New DataView()
            DTProdottiMagazzino.TableName = "MovimentiMagazzino"
            Dv.Table = DTProdottiMagazzino
            Dv.Sort = "Mat_Des ASC"

            For i = 0 To Dv.Count - 1
                Dim traformatoVegetale As MovimentoDiMagazzino = Dv(i).Item("MovimentoMagazzino")
                movimentiMagazzino.Add(traformatoVegetale)
            Next

        End If

        Return movimentiMagazzino

    End Function
End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
