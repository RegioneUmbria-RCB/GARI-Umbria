Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



Public Class CorpiEstranei_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function CorpoEstraneo_Scrivi(ByVal Piva As String,
                                         ByVal Desc_CorpoEstraneo As String,
                                         ByVal Cod_Pericolosita As Integer,
                                         ByVal Desc_Pericolosita As String,
                                         ByVal LimiteMax_Aeroseparatori As Integer,
                                         ByVal LimiteMax_CernitriciOttiche As Integer,
                                         ByVal LimiteMax_CernitaManuale As Integer,
                                         ByVal Validita_Inizio As Date,
                                         ByVal Validita_Fine As Date,
                                         ByRef OUTPUT_Cod_CorpoEstraneo As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        '============================================================================

        Dim objCE_W As AgronicaCoreAnagrafeDAL.CorpiEstranei_W
        Dim objCE_R As AgronicaCoreAnagrafeDAL.CorpiEstranei_R
        Dim objCE_Limiti_W As AgronicaCoreAnagrafeDAL.CorpiEstranei_LimitiPericolosita_W
        Dim objCE_Limiti_R As AgronicaCoreAnagrafeDAL.CorpiEstranei_LimitiPericolosita_R

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True

        Dim Cod_CorpoEstraneo As Integer
        Dim ID As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.CorpiEstranei_W.CorpoEstraneo_Scrivi()"

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)
            ''Se la connessione è chiusa la apro
            'If objParametri.objConnessione Is Nothing Then
            '    objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            '    objParametri.objConnessione.Open()
            '    FlagConnessioneLocale = True
            'End If

            'If objParametri.objTransazione Is Nothing Then
            '    'Inizializzo la transazione
            '    objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
            '    FlagTransazioneLocale = True
            'End If
            '------------------------------


            objCE_R = New AgronicaCoreAnagrafeDAL.CorpiEstranei_R

            'Richiedo un nuovo codice corpo estraneo
            Cod_CorpoEstraneo = objCE_R.Ricava_Nuovo_Cod_CorpoEstraneo(Piva,
                                                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                objParametri)

            OUTPUT_Cod_CorpoEstraneo = Cod_CorpoEstraneo

            objCE_R = Nothing

            objCE_W = New AgronicaCoreAnagrafeDAL.CorpiEstranei_W

            xRisp = objCE_W.Scrivi(Piva,
                                    Cod_CorpoEstraneo,
                                    Desc_CorpoEstraneo,
                                    Validita_Inizio,
                                    Validita_Fine,
                                    objParametri)

            If xRisp Then

                xRisp = False

                objCE_Limiti_R = New AgronicaCoreAnagrafeDAL.CorpiEstranei_LimitiPericolosita_R

                'Richiedo un nuovo codice
                ID = objCE_Limiti_R.Ricava_Nuovo_ID(Piva,
                                                    enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                    objParametri)

                objCE_Limiti_R = Nothing

                objCE_Limiti_W = New AgronicaCoreAnagrafeDAL.CorpiEstranei_LimitiPericolosita_W

                xRisp = objCE_Limiti_W.Scrivi(ID,
                                    Piva,
                                    Cod_CorpoEstraneo,
                                    Cod_Pericolosita,
                                    Desc_Pericolosita,
                                    LimiteMax_Aeroseparatori,
                                    LimiteMax_CernitriciOttiche,
                                    LimiteMax_CernitaManuale,
                                    Validita_Inizio,
                                    Validita_Fine,
                                    objParametri)

                If Not xRisp Then
                    Throw New Exception("Il salvataggio dei limiti di pericolosità del corpo estraneo non è andato a buon fine.")
                End If

            Else
                Throw New Exception("Il salvataggio del corpo estraneo non è andato a buon fine.")
            End If

            '------------------------------
            '------------------------------

            ''Se ho la transazione è stata avviata in questa routine faccio il commit
            'If FlagTransazioneLocale = True Then
            '    objParametri.objTransazione.Commit()
            'End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)



        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = "(Cod_CorpoEstraneo=" & CStr(OUTPUT_Cod_CorpoEstraneo) & ")" &
                              " : " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            ''Chiudo la connessione se è stata aperta in questa routine
            'If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
            '    objParametri.objConnessione.Close()
            'End If
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return xRisp


    End Function


    '============================================================================
    Public Function CorpoEstraneo_Cancella(ByVal Piva As String,
                                           ByVal Cod_CorpoEstraneo As Integer,
                                           ByVal xFiltroAggiuntivo_CE As String,
                                           ByVal xFiltroAggiuntivo_Limiti As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        '============================================================================

        Dim objCE_W As AgronicaCoreAnagrafeDAL.CorpiEstranei_W
        Dim objCE_Limiti_W As AgronicaCoreAnagrafeDAL.CorpiEstranei_LimitiPericolosita_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.CorpiEstranei_W.CorpoEstraneo_Cancella()"

        Try

            '------------------------------
            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
                FlagConnessioneLocale = True

            End If

            If objParametri.objTransazione Is Nothing Then

                'Inizializzo la transazione
                objParametri.objTransazione = objParametri.objConnessione.BeginTransaction

                FlagTransazioneLocale = True

            End If
            '------------------------------


            objCE_W = New AgronicaCoreAnagrafeDAL.CorpiEstranei_W

            xRisp = objCE_W.Cancella(Piva, Cod_CorpoEstraneo,
                                     xFiltroAggiuntivo_CE, objParametri)

            If xRisp Then

                xRisp = False

                objCE_Limiti_W = New AgronicaCoreAnagrafeDAL.CorpiEstranei_LimitiPericolosita_W

                xRisp = objCE_Limiti_W.Cancella_from_ChiaveCE(Piva, _
                                                                Cod_CorpoEstraneo, _
                                                                xFiltroAggiuntivo_Limiti, _
                                                                objParametri)

                If Not xRisp Then
                    Throw New Exception("La cancellazione dei limiti di pericolosità del corpo estraneo non è andata a buon fine.")
                End If

            Else
                Throw New Exception("La cancellazione del corpo estraneo non è andata a buon fine.")
            End If

            '------------------------------

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            If FlagTransazioneLocale Then
                objParametri.objTransazione.Commit()
            End If


        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                objParametri.objTransazione.Rollback()
                objParametri.objTransazione = Nothing
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            If FlagConnessioneLocale AndAlso objParametri.objConnessione IsNot Nothing Then
                objParametri.objConnessione.Close()
            End If

        End Try

        Return xRisp

    End Function

End Class
