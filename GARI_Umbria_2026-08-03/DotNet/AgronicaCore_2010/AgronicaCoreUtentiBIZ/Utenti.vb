Imports System.Data.SqlClient
Imports System.Net.Http
Imports System.Text.RegularExpressions
Imports System.Transactions
Imports Agronica.Helper.Retail
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreModelsSTD.provisioning
Imports AgronicaCoreModelsSTD.utente
Imports AgronicaCoreModelsSTD.Widgets
Imports AgronicaCoreUtentiBIZ.My.Resources
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


Public Class Utenti
    Inherits AgronicaCoreDataProvider.DataProvider

#Region "Visibilita"
    Public Sub InizializzaTabella__tmp_FiltroImpianti(objParametri_server As AgronicaCoreParametri)
        Dim verifica__tmp_FiltroImpianti As New AgronicaCoreVarieDAL.__tmp_FiltroImpianti_R
        If verifica__tmp_FiltroImpianti.VerificaEsistenzaCampoDataCreazione(objParametri_server) Then
            Dim rimuoviRecordVecchiDaTabella As New AgronicaCoreVarieDAL.__tmp_FiltroImpianti_W
            rimuoviRecordVecchiDaTabella.CancellaVecchiRecordPerDataCreazione(objParametri_server)
        End If
    End Sub

    ''' <summary>
    ''' Determines whether the visibility for a specific user and service needs to be recalculated.
    ''' Recalculation is required if the "Pratiche" filter is active, if permissions have never been updated,
    ''' or if the profile was modified after the last permission update.
    ''' </summary>
    ''' <param name="utente_Username">The username of the user whose visibility is being checked.</param>
    ''' <param name="objParametri_Utenti">The application parameters object required for the database read operation.</param>
    ''' <param name="idServizio">The service identifier used to filter the user's profile.</param>
    ''' <returns><c>True</c> if visibility should be recalculated; otherwise, <c>False</c>.</returns>
    Private function ShouldRecalculateVisibility(utente_Username As string, objParametri_Utenti As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri, idServizio As enum_Id_Servizio)
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim filtroPraticheAttivo As Boolean = False
        Dim dataModificaUtentiProfili As New DateTime
        Dim dataUltimoAggiornamentoPermessi As New DateTime?
        Dim DTProfilo = objProfilo.Leggi(utente_Username, idServizio, enumSelezioneVariabile.Selezione_TabellaCompleta,
                "", "", objParametri_Utenti)
        If DTProfilo.Rows.Count > 0 Then
            If UsaCalcoloCombinatoStd(objParametri_Server) Then
                filtroPraticheAttivo = DTProfilo.Rows(0)("Filtro_Pratiche_Attivo") = True
            End If
            dataModificaUtentiProfili = DTProfilo.Rows(0).Field(Of DateTime)("Data_Modifica")
            dataUltimoAggiornamentoPermessi = DTProfilo.Rows(0).Field(Of DateTime?)("DataUltimoRiportoUtentiVisibilitaAppoggio")
        End If
        Return filtroPraticheAttivo OrElse Not dataUltimoAggiornamentoPermessi.HasValue OrElse dataUltimoAggiornamentoPermessi.Value < dataModificaUtentiProfili
    End function

    Public Sub InizializzaTabellaUtentiVisibilitaAppoggio(utente_Username As String, idServizio As Integer, objParametri_server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)
        Dim objConf_Siti_R As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim boolTester As Boolean = False
        Dim sincronizzaInBackground As String = objConf_Siti_R.Leggi_Valore(0, "Utenti_Visibilia_Appoggio_In_BackGround", "", "", objParametri_server)
        If Not String.IsNullOrEmpty(sincronizzaInBackground) Then
            Boolean.TryParse(sincronizzaInBackground, boolTester)
        End If
        If boolTester Then
            Return
        End If
        If UsaCalcoloCombinatoStd(objParametri_server) Then
            InizializzaVisibilitaAppoggio_Combinato_Std(utente_Username, idServizio, objParametri_server, objParametri_Utenti)
        Else
            InizializzaVisibilitaAppoggio(utente_Username, objParametri_server, objParametri_Utenti, idServizio)
        End If
    End Sub

    Public Sub InizializzaTabellaUtentiVisibilitaAppoggio_EF(utente_Username As String, idServizio As Integer, objParametri_server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)
        '---------------------------------------------------------------
        '(17/11/2014) gestione tabella Utenti_Visibilita_Appoggio ------
        'ricarico i dati che può visualizzare
        '---------------------------------------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim messaggioErrore As String = ""

        Try
            'leggo dalla tabella profili utenti la visibilita dell'utente
            Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            Dim objProfiloW As New AgronicaCoreUtentiDAL.Utenti_Profili_Write

            If not objProfilo.HasFullVisibility(utente_Username, objParametri_Utenti) Then
                If ShouldRecalculateVisibility(utente_Username, objParametri_Utenti, objParametri_server, idServizio) Then
                    Dim Sql_Permessi = objProfilo.Leggi_FiltroUtenteSQL(utente_Username, idServizio, "", "", objParametri_Utenti)

                    Dim classJoin As New JoinFiltrone
                    classJoin.bGerarchiaImprese = True
                    classJoin.bCentriAziendali = True

                    Dim Filtrone As New AgronicaCoreUtility.Filtrone

                    Dim objUtentiVisibilita_W As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
                    Dim objUtentiVisibilita_R As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    'INSERIMENTO IMPRESE VISIBILI

                    Dim PivaSuperUser = objParametri_server.PivaSuperUser
                    Dim Entita_Cod_Impresa = 1
                    Dim Entita_Cod_Centro = 2

                    Filtrone.MantieniParametri = True
                    Dim sqlFiltroneImprese = Filtrone.CreaStringaQueryPerDTFiltrone(objParametri_server, Sql_Permessi, enum_TipoSelect_FiltroneSuperNova.Imprese, " ", classJoin, True, False)
                    'Dim DTImprese = Filtrone.CreaDTFiltrone(objParametri_server, Sql_Permessi, enum_TipoSelect_FiltroneSuperNova.Imprese, "", classJoin)
                    objUtentiVisibilita_R.SettaParametriPrecedenti(Filtrone.DammiParametriCollezionati)
                    Dim Utenti_Visibilita_Appoggio_Imprese As DataTable = objUtentiVisibilita_R.CreaDTFiltroneImprese(PivaSuperUser, utente_Username, Entita_Cod_Impresa, sqlFiltroneImprese, objParametri_server)

                    Dim sqlNewImprese = AgronicaCoreVisibilitaStd.VisibilitaQueryBuilder.BuildQueryImprese(Sql_Permessi, AgronicaCoreVisibilitaStd.ModalitaQuery.Full)
                    AgronicaCoreUtility.VisibilitaDiffLogger.LogDiff("_EF", "Full", utente_Username, "Imprese", sqlFiltroneImprese, sqlNewImprese, Utenti_Visibilita_Appoggio_Imprese.Rows.Count)

                    'INSERIMENTO CENTRI VISIBILI
                    Filtrone.SvuotaTuttiIParametri()
                    Dim sqlFiltroneCentri = Filtrone.CreaStringaQueryPerDTFiltrone(objParametri_server, Sql_Permessi, enum_TipoSelect_FiltroneSuperNova.CentriAziendali, " ", classJoin, True, False)
                    objUtentiVisibilita_R.SettaParametriPrecedenti(Filtrone.DammiParametriCollezionati)
                    Dim Utenti_Visibilita_Appoggio_Centri As DataTable = objUtentiVisibilita_R.CreaDTFiltroneCentri(PivaSuperUser, utente_Username, Entita_Cod_Centro, sqlFiltroneCentri, objParametri_server)

                    Dim sqlNewCentri = AgronicaCoreVisibilitaStd.VisibilitaQueryBuilder.BuildQueryCentri(Sql_Permessi, AgronicaCoreVisibilitaStd.ModalitaQuery.Full)
                    AgronicaCoreUtility.VisibilitaDiffLogger.LogDiff("_EF", "Full", utente_Username, "Centri", sqlFiltroneCentri, sqlNewCentri, Utenti_Visibilita_Appoggio_Centri.Rows.Count)

                    Try
                        ''Apro la connessione al DB
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                            FlagConnessioneLocale, FlagTransazioneLocale, objParametri_server, System.Data.IsolationLevel.ReadUncommitted)


                        objUtentiVisibilita_W.Cancella(0, "", objParametri_server)

                        Dim copyOptions As SqlBulkCopyOptions = New SqlBulkCopyOptions()
                        Dim externalTransaction As SqlTransaction = objParametri_server.objTransazione

                        Dim sqlBulkCopy = New SqlBulkCopy(objParametri_server.objConnessione, copyOptions, externalTransaction)

                        'getConnectionStringFromOleToSql(objParametri_server.StringaConnessione)

                        sqlBulkCopy.DestinationTableName = "dbo.Utenti_Visibilita_Appoggio"
                        sqlBulkCopy.WriteToServer(Utenti_Visibilita_Appoggio_Imprese)
                        sqlBulkCopy.WriteToServer(Utenti_Visibilita_Appoggio_Centri)
                        'elimino tutti i record relativi all'utente che entra

                        'Se l'aggiornamento sulla utenti_profili (che è atomica) non va a buon fine devo invalidare anche la transazione sul db_server.
                        Dim risAggProfili = objProfiloW.ModificaDataUtentiVisibilitaAppoggio(utente_Username, idServizio, DateTime.Now, objParametri_Utenti)
                        If risAggProfili = False Then
                            Throw New Exception("Errore nell'aggiornamento data ultimo riporto utenti visibilita appoggio")
                        End If

                        ''Chiudo la connessione al DB
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)

                    Catch ex As Exception

                        If Not objParametri_server.objTransazione Is Nothing Then
                            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server)
                        End If
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)
                        Throw ex
                    End Try

                End If
            Else

                'elimino tutti i record relativi all'utente che entra anche se l'utente ha visibilità totale
                'per azzerarla nel caso la totale sia stata data in seguito ad una visibilità parziale
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
                objUtentiVisibilita.Cancella(0, "", objParametri_server)

            End If
        Catch ex As Exception
            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then
                Messaggio &= "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio &= vbCrLf
                Messaggio &= messaggioErrore
                Messaggio &= vbCrLf
                Messaggio &= "Ritentare il salvataggio dopo la correzione ..."
            End If
            'AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)
            Throw New Exception(messaggioErrore, ex)
        End Try

    End Sub

    Public Sub InizializzaTabellaUtentiVisibilitaAppoggio_GUID(utente_Username As String, idServizio As Integer, objParametri_server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim messaggioErrore As String = ""
        Try
            'leggo dalla tabella profili utenti la visibilita dell'utente
            Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read

            If not objProfilo.HasFullVisibility(utente_Username, objParametri_Utenti) Then
                If ShouldRecalculateVisibility(utente_Username, objParametri_Utenti, objParametri_Server, idServizio) Then
                    Dim Filtrone As New AgronicaCoreUtility.Filtrone
                    Dim objUtentiVisibilita_W As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
                    Dim objUtentiVisibilita_R As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim objProfiloW As New AgronicaCoreUtentiDAL.Utenti_Profili_Write
                    Dim classJoin As New JoinFiltrone
                    classJoin.bGerarchiaImprese = True
                    classJoin.bCentriAziendali = True

                    dim Sql_Permessi = objProfilo.Leggi_FiltroUtenteSQL(utente_Username, idServizio, "", "", objParametri_Utenti)
                    Dim guid_str = Guid.NewGuid().ToString.Replace("-", "_")

                    Try
                        ''Apro la connessione al DB
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                            FlagConnessioneLocale, FlagTransazioneLocale, objParametri_server, System.Data.IsolationLevel.ReadUncommitted)

                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)

                        'INSERIMENTO IMPRESE VISIBILI
                        Dim PivaSuperUser = objParametri_server.PivaSuperUser
                        Dim Entita_Cod_Impresa = 1
                        Dim Entita_Cod_Centro = 2

                        Filtrone.MantieniParametri = True
                        Dim sqlFiltroneImprese = Filtrone.CreaStringaQueryPerDTFiltrone(objParametri_server, Sql_Permessi, enum_TipoSelect_FiltroneSuperNova.Imprese, " ", classJoin, True, False)
                        objUtentiVisibilita_R.SettaParametriPrecedenti(Filtrone.DammiParametriCollezionati)
                        objUtentiVisibilita_R.PopolaTabellaTemporaneaDTFiltroneImprese(guid_str, PivaSuperUser, utente_Username, Entita_Cod_Impresa, sqlFiltroneImprese, objParametri_server)

                        'INSERIMENTO CENTRI VISIBILI
                        Filtrone.SvuotaTuttiIParametri()
                        Dim sqlFiltroneCentri = Filtrone.CreaStringaQueryPerDTFiltrone(objParametri_server, Sql_Permessi, enum_TipoSelect_FiltroneSuperNova.CentriAziendali, " ", classJoin, True, False)
                        objUtentiVisibilita_R.SettaParametriPrecedenti(Filtrone.DammiParametriCollezionati)
                        objUtentiVisibilita_R.PopolaTabellaTemporaneaDTFiltroneCentri(guid_str, PivaSuperUser, utente_Username, Entita_Cod_Centro, sqlFiltroneCentri, objParametri_server)

                        objUtentiVisibilita_W.PopolaDaTabellaTemporanea(guid_str, PivaSuperUser, utente_Username, objParametri_server)
                        objUtentiVisibilita_W.CancellaDaTabellaTemporanea(guid_str, PivaSuperUser, utente_Username, objParametri_server)

                        'Se l'aggiornamento sulla utenti_profili (che è atomica) non va a buon fine devo invalidare anche la transazione sul db_server.
                        Dim risAggProfili = objProfiloW.ModificaDataUtentiVisibilitaAppoggio(utente_Username, idServizio, DateTime.Now, objParametri_Utenti)
                        If risAggProfili = False Then
                            Throw New Exception("Errore nell'aggiornamento data ultimo riporto utenti visibilita appoggio")
                        End If

                        ''Chiudo la connessione al DB
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)

                    Catch ex As Exception
                        If objParametri_server.objTransazione Isnot Nothing Then
                            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server)
                        End If
                        Throw ex
                    Finally
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)
                    End Try

                End If
            Else

                'elimino tutti i record relativi all'utente che entra anche se l'utente ha visibilità totale
                'per azzerarla nel caso la totale sia stata data in seguito ad una visibilità parziale
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
                objUtentiVisibilita.Cancella(0, "", objParametri_server)

            End If
        Catch ex As Exception
            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then
                Messaggio &= "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio &= vbCrLf
                Messaggio &= messaggioErrore
                Messaggio &= vbCrLf
                Messaggio &= "Ritentare il salvataggio dopo la correzione ..."
            End If
            Throw New Exception(messaggioErrore, ex)
        End Try

    End Sub

    Public Function Inizializza_Tabella_Temp_UtentiVisibilitaAppoggio(
            utente_Username As String,
            Sql_Permessi As String,
            dataModificaUtentiProfili As DateTime,
            dataUltimoAggiornamentoPermessi As DateTime?,
            idServizio As Integer,
            objParametri_server As AgronicaCoreParametri,
            objParametri_Utenti As AgronicaCoreParametri) As String

        'leggo dalla tabella profili utenti la visibilita dell'utente
        Dim objProfiloW As New AgronicaCoreUtentiDAL.Utenti_Profili_Write
        Dim guid_str As String = String.Empty

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        If Not dataUltimoAggiornamentoPermessi.HasValue OrElse dataUltimoAggiornamentoPermessi.Value < dataModificaUtentiProfili Then
            Dim classJoin As New JoinFiltrone
            classJoin.bGerarchiaImprese = True
            classJoin.bCentriAziendali = True

            Dim Filtrone As New AgronicaCoreUtility.Filtrone

            Dim objUtentiVisibilita_W As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
            Dim objUtentiVisibilita_R As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R

            guid_str = Guid.NewGuid().ToString.Replace("-", "_")

            Try

                ''Apro la connessione al DB
                'AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                '                                                            FlagTransazioneLocale,
                '                                                            objParametri_server, System.Data.IsolationLevel.ReadUncommitted)

                'AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                '                                                            FlagTransazioneLocale,
                '                                                            objParametri_Utenti, System.Data.IsolationLevel.ReadUncommitted)


                'AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)
                'AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Utenti)

                Utility.VerificaApriConnessione(objParametri_server, FlagConnessioneLocale)
                Utility.VerificaApriConnessione(objParametri_Utenti, FlagConnessioneLocale)

                objUtentiVisibilita_W.CreaTabellaTemporaneaUtenti_Visibilita_Appoggio(guid_str, objParametri_server)

                'INSERIMENTO IMPRESE VISIBILI
                Dim PivaSuperUser = objParametri_server.PivaSuperUser
                Dim Entita_Cod_Impresa = 1
                Dim Entita_Cod_Centro = 2

                Filtrone.MantieniParametri = True
                Dim sqlFiltroneImprese = Filtrone.CreaStringaQueryPerDTFiltrone(objParametri_server, Sql_Permessi, enum_TipoSelect_FiltroneSuperNova.Imprese_Visibilita_Appoggio, " ", classJoin, True, False)
                objUtentiVisibilita_R.SettaParametriPrecedenti(Filtrone.DammiParametriCollezionati)
                objUtentiVisibilita_R.PopolaTabellaTemporaneaDTFiltroneImprese(guid_str, PivaSuperUser, utente_Username, Entita_Cod_Impresa, sqlFiltroneImprese, objParametri_server)


                'INSERIMENTO CENTRI VISIBILI
                Filtrone.SvuotaTuttiIParametri()
                Dim sqlFiltroneCentri = Filtrone.CreaStringaQueryPerDTFiltrone(objParametri_server, Sql_Permessi, enum_TipoSelect_FiltroneSuperNova.CentriAziendali_Visibilita_Appoggio, " ", classJoin, True, False)
                objUtentiVisibilita_R.SettaParametriPrecedenti(Filtrone.DammiParametriCollezionati)
                objUtentiVisibilita_R.PopolaTabellaTemporaneaDTFiltroneCentri(guid_str, PivaSuperUser, utente_Username, Entita_Cod_Centro, sqlFiltroneCentri, objParametri_server)

            Catch ex As Exception

                If Not objParametri_server.objTransazione Is Nothing Then
                    'AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server)
                End If

                Throw ex
            Finally

                'AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)
                'objUtentiVisibilita_W.CancellaTabellaTemporaneaUtenti_Visibilita_Appoggio(guid_str, objParametri_server)

            End Try

        End If

        Return guid_str

    End Function

    Public Sub Finalizza_Tabella_Temp_UtentiVisibilitaAppoggio(
            utente_UserName As String,
            nomeTabellaTemp As String,
            objParametri_server As AgronicaCoreParametri,
            objParametri_Utenti As AgronicaCoreParametri)

        Dim PivaSuperUser = objParametri_server.PivaSuperUser
        Dim objUtentiVisibilita_W As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
        Dim objProfiloW As New AgronicaCoreUtentiDAL.Utenti_Profili_Write

        Try



            objUtentiVisibilita_W.PopolaDaTabellaTemporanea(nomeTabellaTemp, PivaSuperUser, utente_UserName, objParametri_server)
            objUtentiVisibilita_W.CancellaDaTabellaTemporanea(nomeTabellaTemp, PivaSuperUser, utente_UserName, objParametri_server)

            ''Se l'aggiornamento sulla utenti_profili (che è atomica) non va a buon fine devo invalidare anche la transazione sul db_server.
            Dim risAggProfili = objProfiloW.ModificaDataUtentiVisibilitaAppoggio(utente_UserName, 5, DateTime.Now, objParametri_Utenti)
            If risAggProfili = False Then
                Throw New Exception("Errore nell'aggiornamento data ultimo riporto utenti visibilita appoggio")
            End If

        Catch ex As Exception
            Throw ex

        Finally
            'objUtentiVisibilita_W.CancellaTabellaTemporaneaUtenti_Visibilita_Appoggio(nomeTabellaTemp, objParametri_server)

            Utility.VerificaChiudiConnessione(objParametri_server, True)
            Utility.VerificaChiudiConnessione(objParametri_Utenti, True)
        End Try

    End Sub

    Public Sub InizializzaTabellaUtentiVisibilitaAppoggio_GUID_NoTransaction(utente_Username As String, idServizio As Integer, objParametri_server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        '''(09/11/2023) introduce gestione visibilità nulla:
        ''' Se il filtro visibilità in Utenti_Profili contiene la piva fittizia qui indicata, riconosco che l'utente 
        ''' ha visibilità nulla e inserisco un record fasullo nella tabella Utenti_Visibilità_Appoggio
        Dim pivaFittizia = "###########"

        Try
            'leggo dalla tabella profili utenti la visibilita dell'utente
            Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            Dim objProfiloW As New AgronicaCoreUtentiDAL.Utenti_Profili_Write
            Dim objUtentiVisibilita_W As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
            Dim objUtentiVisibilita_R As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R

            If not objProfilo.HasFullVisibility(utente_Username, objParametri_Utenti) Then
                If ShouldRecalculateVisibility(utente_Username, objParametri_Utenti, objParametri_server, idServizio) Then
                    Dim classJoin As New JoinFiltrone
                    classJoin.bGerarchiaImprese = True
                    classJoin.bCentriAziendali = True
                    Dim Sql_Permessi = objProfilo.Leggi_FiltroUtenteSQL(utente_Username, idServizio, "", "", objParametri_Utenti)

                    Dim Filtrone As New AgronicaCoreUtility.Filtrone
                    Dim guid_str = Guid.NewGuid().ToString.Replace("-", "_")

                    Try
                        ''Apro la connessione al DB
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                            FlagConnessioneLocale, FlagTransazioneLocale, objParametri_server, System.Data.IsolationLevel.ReadUncommitted)

                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)

                        If  Not Sql_Permessi.Contains(pivaFittizia) Then
                            objUtentiVisibilita_W.CreaTabellaTemporaneaUtenti_Visibilita_Appoggio(guid_str, objParametri_server)

                            'INSERIMENTO IMPRESE VISIBILI
                            Dim PivaSuperUser = objParametri_server.PivaSuperUser
                            Dim Entita_Cod_Impresa = 1
                            Dim Entita_Cod_Centro = 2

                            Filtrone.MantieniParametri = True
                            Dim sqlFiltroneImprese = Filtrone.CreaStringaQueryPerDTFiltrone(objParametri_server, Sql_Permessi, enum_TipoSelect_FiltroneSuperNova.Imprese_Visibilita_Appoggio, " ", classJoin, True, False)

                            objUtentiVisibilita_R.SettaParametriPrecedenti(Filtrone.DammiParametriCollezionati)
                            Dim DTImpreseUva As DataTable = objUtentiVisibilita_R.PopolaTabellaTemporaneaDTFiltroneImprese(guid_str, PivaSuperUser, utente_Username, Entita_Cod_Impresa, sqlFiltroneImprese, objParametri_server)

                            Dim sqlNewImprese = AgronicaCoreVisibilitaStd.VisibilitaQueryBuilder.BuildQueryImprese(Sql_Permessi, AgronicaCoreVisibilitaStd.ModalitaQuery.UvaLean)
                            AgronicaCoreUtility.VisibilitaDiffLogger.LogDiff("_GUID_NoTransaction", "UvaLean", utente_Username, "Imprese", sqlFiltroneImprese, sqlNewImprese, If(DTImpreseUva Is Nothing, 0, DTImpreseUva.Rows.Count))

                            'INSERIMENTO CENTRI VISIBILI
                            Filtrone.SvuotaTuttiIParametri()
                            Dim sqlFiltroneCentri = Filtrone.CreaStringaQueryPerDTFiltrone(objParametri_server, Sql_Permessi, enum_TipoSelect_FiltroneSuperNova.CentriAziendali_Visibilita_Appoggio, " ", classJoin, True, False)
                            objUtentiVisibilita_R.SettaParametriPrecedenti(Filtrone.DammiParametriCollezionati)
                            Dim DTCentriUva As DataTable = objUtentiVisibilita_R.PopolaTabellaTemporaneaDTFiltroneCentri(guid_str, PivaSuperUser, utente_Username, Entita_Cod_Centro, sqlFiltroneCentri, objParametri_server)

                            Dim sqlNewCentri = AgronicaCoreVisibilitaStd.VisibilitaQueryBuilder.BuildQueryCentri(Sql_Permessi, AgronicaCoreVisibilitaStd.ModalitaQuery.UvaLean)
                            AgronicaCoreUtility.VisibilitaDiffLogger.LogDiff("_GUID_NoTransaction", "UvaLean", utente_Username, "Centri", sqlFiltroneCentri, sqlNewCentri, If(DTCentriUva Is Nothing, 0, DTCentriUva.Rows.Count))

                            objUtentiVisibilita_W.PopolaDaTabellaTemporanea(guid_str, PivaSuperUser, utente_Username, objParametri_server)
                            objUtentiVisibilita_W.CancellaDaTabellaTemporanea(guid_str, PivaSuperUser, utente_Username, objParametri_server)

                            'Se l'aggiornamento sulla utenti_profili (che è atomica) non va a buon fine devo invalidare anche la transazione sul db_server.
                            Dim risAggProfili = objProfiloW.ModificaDataUtentiVisibilitaAppoggio(utente_Username, idServizio, DateTime.Now, objParametri_Utenti)
                            If risAggProfili = False Then
                                Throw New Exception("Errore nell'aggiornamento data ultimo riporto utenti visibilita appoggio")
                            End If

                        End If
                    Catch ex As Exception

                        If objParametri_server.objTransazione IsNot Nothing Then
                            'AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server)
                        End If
                        Throw ex

                    Finally
                        '''Chiudo la connessione al DB
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)
                        'objUtentiVisibilita_W.CancellaTabellaTemporaneaUtenti_Visibilita_Appoggio(guid_str, objParametri_server)
                    End Try

                End If

            Else
                'elimino tutti i record relativi all'utente che entra anche se l'utente ha visibilità totale
                'per azzerarla nel caso la totale sia stata data in seguito ad una visibilità parziale
                objUtentiVisibilita_W.Cancella(0, "", objParametri_server, utente_Username)

            End If

        Catch ex As Exception
            Dim messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
            If messaggioErrore <> "" Then
                Dim Messaggio = "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio &= vbCrLf & messaggioErrore & vbCrLf
                Messaggio &= "Ritentare il salvataggio dopo la correzione ..."
                messaggioErrore = Messaggio
            End If
            Throw New Exception(messaggioErrore, ex)
        End Try
    End Sub

    ''' <summary>
    ''' Inizializza la tabella <tt>Utenti_Visibilita_Appoggio</tt> per l'username specificato.
    ''' </summary>
    ''' <remarks>
    ''' 23/01/24 - introdotto flag <tt>Utenti_Visibilia_Appoggio_Da_Capostipiti</tt> in <tt>Configurazione_Siti</tt>
    ''' per gestione scrittura tabella <tt>Utenti_Visibilita_Appoggio</tt>. Se valorizzato a <tt>1</tt> la scrittura
    ''' della tabella avviene prendendo in considerazione le pive specificate nel filtro xml della tabella
    ''' <tt>Utenti_Profili</tt> e usandole come capostipiti per calcolare ricorsivamente la gerarchia delle imprese
    ''' visibili.
    ''' </remarks>
    ''' <param name="username"></param>
    ''' <param name="objPServer"></param>
    ''' <param name="objPUtenti"></param>
    ''' <param name="idServizio"></param>
    Public Sub InizializzaVisibilitaAppoggio(
        username As String,
        objPServer As AgronicaCoreParametri,
        objPUtenti As AgronicaCoreParametri,
        Optional idServizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline
    )
        Dim config As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim usaCapostipiti As String = config.Leggi_Valore(
            Sito_Cod:=0, "Utenti_Visibilia_Appoggio_Da_Capostipiti",
            xFiltroAggiuntivo:="", xOrderBy:="", objPServer
        )
        If String.IsNullOrWhiteSpace(usaCapostipiti) OrElse usaCapostipiti = "1" Then
            PopolaVisibilitaAppoggioXUtente(username, objPServer, objPUtenti)
        Else
            InizializzaTabellaUtentiVisibilitaAppoggio_GUID_NoTransaction(
                username, idServizio,
                objPServer, objPUtenti
            )
        End If
    End Sub

    Public Sub PopolaVisibilitaAppoggioXUtente(
        username As String,
        objPServer As AgronicaCoreParametri, objPUtenti As AgronicaCoreParametri
    )
        Dim bizVisibilita As New AgronicaCoreUtentiBIZ.Utenti_Visibilita
        Dim dalVisibilitaAppoggio As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
        Dim strsSql As String = String.Empty

        Try
            Dim pive = bizVisibilita.LeggiPiveCapostipiti(username, objPUtenti)
            dalVisibilitaAppoggio.Cancella(Entita_Cod:=0, xFiltroAggiuntivo:="", objPServer, username)
            dalVisibilitaAppoggio.PopolaConGerarchia(username, pive, objPServer, strsSql)
        Catch ex As Exception
            Dim nMsg = "ERROR for user """ & username & """: " &
                    AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
            Throw New Exception(nMsg)
        End Try
    End Sub
#End Region

#Region "Visibilita Combinata Std (DS01-BL)"

    ''' <summary>
    ''' Flag che instrada l'entry-point <see cref="InizializzaTabellaUtentiVisibilitaAppoggio"/>
    ''' verso il nuovo orchestrator basato su <c>AgronicaCoreVisibilitaStd</c> (gerarchia + pratiche
    ''' combinate AND/OR) anziche' la pipeline legacy <see cref="InizializzaVisibilitaAppoggio"/>.
    ''' </summary>
    Private Function UsaCalcoloCombinatoStd(objParametri_server As AgronicaCoreParametri) As Boolean
        Dim config As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim usaCapostipiti As String = config.Leggi_Valore(
            Sito_Cod:=0, "Utenti_Visibilita_Calcolo_Combinato_Pratiche",
            xFiltroAggiuntivo:="", xOrderBy:="", objParametri_server
        )
        Return usaCapostipiti.Equals("1")
    End Function

    ''' <summary>
    ''' Orchestrator DS01-BL: calcola la visibilita' combinata (gerarchia + pratiche con
    ''' operatore AND/OR letto dal profilo) e popola la tabella <c>Utenti_Visibilita_Appoggio</c>
    ''' via <see cref="SqlBulkCopy"/>. Usa esclusivamente builder std in modalita'
    ''' <see cref="AgronicaCoreVisibilitaStd.ModalitaQuery.UvaLean"/>.
    ''' </summary>
    Public Sub InizializzaVisibilitaAppoggio_Combinato_Std(
        utente_Username As String,
        idServizio As Integer,
        objParametri_server As AgronicaCoreParametri,
        objParametri_Utenti As AgronicaCoreParametri
    )
        Dim NomeRoutine As String = "AgronicaCoreUtentiBIZ.Utenti.InizializzaVisibilitaAppoggio_Combinato_Std()"
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            Dim profilo = LeggiProfiloFiltri_Std(utente_Username, idServizio, objParametri_Utenti)
            Dim daCapostipiti As Boolean = LeggiFlagCapostipiti_Std(objParametri_server)

            Dim piveGerarchia As HashSet(Of String) = Nothing
            Dim centriGerarchia As HashSet(Of Tuple(Of String, Integer)) = Nothing
            CalcolaGerarchia_Std(
                profilo.Descrizione_1, profilo.Descrizione_2, daCapostipiti,
                objParametri_server, piveGerarchia, centriGerarchia
            )

            Dim pivePratiche As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Dim centriPraticheOnly As New HashSet(Of Tuple(Of String, Integer))()

            If profilo.FiltroPraticheAttivo Then
                centriPraticheOnly = LeggiImpreseCentriDaPratiche(utente_Username, objParametri_Utenti, objParametri_server)
                pivePratiche = centriPraticheOnly.Select(Function(t) t.Item1).ToHashSet()
            End If

            Dim piveFinali As HashSet(Of String)
            Dim centriFinali As HashSet(Of Tuple(Of String, Integer))
            If profilo.FiltroPraticheAttivo Then
                piveFinali = AgronicaCoreVisibilitaStd.VisibilitaCombinator.Combina(piveGerarchia, pivePratiche, profilo.OperatoreFiltri)
                centriFinali = AgronicaCoreVisibilitaStd.VisibilitaCombinator.CombinaCentri(centriGerarchia, centriPraticheOnly, pivePratiche, profilo.OperatoreFiltri)
            Else
                piveFinali = piveGerarchia
                centriFinali = centriGerarchia
            End If

            Dim dtImprese = BuildDataTableUVAImprese_Std(objParametri_server.PivaSuperUser, utente_Username, piveFinali)
            Dim dtCentri = BuildDataTableUVACentri_Std(objParametri_server.PivaSuperUser, utente_Username, centriFinali)

            Try
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                    FlagConnessioneLocale, FlagTransazioneLocale,
                    objParametri_server, System.Data.IsolationLevel.ReadUncommitted
                )

                Dim uvaW As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
                uvaW.Cancella(0, "", objParametri_server, utente_Username)

                Dim externalTransaction As SqlTransaction = objParametri_server.objTransazione
                Using bulk As New SqlBulkCopy(objParametri_server.objConnessione, New SqlBulkCopyOptions(), externalTransaction)
                    bulk.DestinationTableName = "dbo.Utenti_Visibilita_Appoggio"
                    If dtImprese.Rows.Count > 0 Then bulk.WriteToServer(dtImprese)
                    If dtCentri.Rows.Count > 0 Then bulk.WriteToServer(dtCentri)
                End Using

                Dim profiloW As New AgronicaCoreUtentiDAL.Utenti_Profili_Write
                If Not profiloW.ModificaDataUtentiVisibilitaAppoggio(utente_Username, idServizio, DateTime.Now, objParametri_Utenti) Then
                    Throw New Exception("Errore aggiornamento DataUltimoRiportoUtentiVisibilitaAppoggio")
                End If

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)
            Catch
                If Not objParametri_server.objTransazione Is Nothing Then
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server)
                End If
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)
                Throw
            End Try
        Catch ex As Exception
            Scrivi_LOG(objParametri_server, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message, ex)
        End Try
    End Sub

    ''' <summary>
    ''' Compute-only variant of <see cref="InizializzaVisibilitaAppoggio_Combinato_Std"/>
    ''' for the parallel background synchronizer (GSB).
    ''' Returns two DataTables ready for <see cref="SqlBulkCopy"/> without opening a write connection.
    ''' Thread-safe when called with independent deep-copied parameter instances per parallel task.
    ''' </summary>
    ''' <param name="daCapostipiti">Pre-read flag from <c>Utenti_Visibilia_Appoggio_Da_Capostipiti</c>;
    ''' compute once in the caller and pass to each parallel call to avoid repeated DB reads.</param>
    ''' <returns>Tuple of (dtImprese, dtCentri). Both tables may have 0 rows when the user has full visibility.</returns>
    Public Function CalcolaVisibilitaCombinata_Std(
        utente_Username As String,
        idServizio As Integer,
        daCapostipiti As Boolean,
        daCombinatoPratiche As Boolean,
        objParametri_server As AgronicaCoreParametri,
        objParametri_Utenti As AgronicaCoreParametri
    ) As Tuple(Of DataTable, DataTable)
        Dim NomeRoutine As String = "AgronicaCoreUtentiBIZ.Utenti.CalcolaVisibilitaCombinata_Std()"
        Try
            Dim profilo = LeggiProfiloFiltri_Std(utente_Username, idServizio, objParametri_Utenti)

            Dim piveGerarchia As HashSet(Of String) = Nothing
            Dim centriGerarchia As HashSet(Of Tuple(Of String, Integer)) = Nothing
            CalcolaGerarchia_Std(
                profilo.Descrizione_1, profilo.Descrizione_2, daCapostipiti,
                objParametri_server, piveGerarchia, centriGerarchia
            )

            Dim pivePratiche As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Dim centriPraticheOnly As New HashSet(Of Tuple(Of String, Integer))()

            If daCombinatoPratiche AndAlso profilo.FiltroPraticheAttivo Then
                centriPraticheOnly = LeggiImpreseCentriDaPratiche(utente_Username, objParametri_Utenti, objParametri_server)
                pivePratiche = centriPraticheOnly.Select(Function(t) t.Item1).ToHashSet()
            End If

            Dim piveFinali As HashSet(Of String)
            Dim centriFinali As HashSet(Of Tuple(Of String, Integer))
            If daCombinatoPratiche AndAlso profilo.FiltroPraticheAttivo Then
                piveFinali = AgronicaCoreVisibilitaStd.VisibilitaCombinator.Combina(piveGerarchia, pivePratiche, profilo.OperatoreFiltri)
                centriFinali = AgronicaCoreVisibilitaStd.VisibilitaCombinator.CombinaCentri(centriGerarchia, centriPraticheOnly, pivePratiche, profilo.OperatoreFiltri)
            Else
                piveFinali = piveGerarchia
                centriFinali = centriGerarchia
            End If

            Return Tuple.Create(
                BuildDataTableUVAImprese_Std(objParametri_server.PivaSuperUser, utente_Username, piveFinali),
                BuildDataTableUVACentri_Std(objParametri_server.PivaSuperUser, utente_Username, centriFinali)
            )
        Catch ex As Exception
            Scrivi_LOG(objParametri_server, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message, ex)
        End Try
    End Function

    Private Class ProfiloFiltri_Std
        Public Property Descrizione_1 As String = ""
        Public Property Descrizione_2 As String = ""
        Public Property FiltroPraticheAttivo As Boolean = False
        Public Property OperatoreFiltri As String = "OR"
    End Class

    Private Function LeggiProfiloFiltri_Std(
        utente_Username As String, idServizio As Integer,
        objParametri_Utenti As AgronicaCoreParametri
    ) As ProfiloFiltri_Std
        Dim profilo As New ProfiloFiltri_Std
        Dim profiloR As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim dt = profiloR.Leggi(
            utente_Username, idServizio,
            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
            "", "", objParametri_Utenti
        )
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            profilo.Descrizione_1 = If(dt.Rows(0)("Descrizione_1"), "").ToString()
            profilo.Descrizione_2 = If(dt.Rows(0)("Descrizione_2"), "").ToString()
            If dt.Columns.Contains("Filtro_Pratiche_Attivo") AndAlso dt.Rows(0)("Filtro_Pratiche_Attivo") IsNot DBNull.Value Then
                profilo.FiltroPraticheAttivo = Convert.ToBoolean(dt.Rows(0)("Filtro_Pratiche_Attivo"))
            End If
            If dt.Columns.Contains("Operatore_Filtri") AndAlso dt.Rows(0)("Operatore_Filtri") IsNot DBNull.Value Then
                Dim raw = Convert.ToString(dt.Rows(0)("Operatore_Filtri"))
                If Not String.IsNullOrWhiteSpace(raw) Then profilo.OperatoreFiltri = raw.Trim().ToUpperInvariant()
            End If
        End If
        Return profilo
    End Function

    Private Function LeggiFlagCapostipiti_Std(objParametri_server As AgronicaCoreParametri) As Boolean
        Dim cfg As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim raw = cfg.Leggi_Valore(0, "Utenti_Visibilia_Appoggio_Da_Capostipiti", "", "", objParametri_server)
        Return String.IsNullOrWhiteSpace(raw) OrElse raw.Trim() = "1"
    End Function

    Private Sub CalcolaGerarchia_Std(
        descrizione1 As String, descrizione2 As String, daCapostipiti As Boolean,
        objParametri_server As AgronicaCoreParametri,
        ByRef pive As HashSet(Of String),
        ByRef centri As HashSet(Of Tuple(Of String, Integer))
    )
        pive = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        centri = New HashSet(Of Tuple(Of String, Integer))()

        If daCapostipiti Then
            Dim piveSeed = AgronicaCoreVisibilitaStd.CapostipitiHelper.ExtractPive(descrizione1)
            If AgronicaCoreVisibilitaStd.CapostipitiHelper.IsVisibilitaTotaleONulla(piveSeed) Then Return

            Dim build = AgronicaCoreVisibilitaStd.CapostipitiQueryBuilder.BuildQueryEspansioneGerarchia(piveSeed)
            If build Is Nothing Then Return

            Dim dt = EseguiQuery_Lettura(objParametri_server, build.Sql, build.Parametri, "CalcolaGerarchia_Std.Capostipiti")
            ReadPive_Std(dt, pive)
            For Each row As DataRow In dt.Rows
                If row("Piva") Is DBNull.Value OrElse row("Sa_Cod") Is DBNull.Value Then Continue For
                Dim p = Convert.ToString(row("Piva"))
                If String.IsNullOrWhiteSpace(p) Then Continue For
                Dim sa = Convert.ToInt32(row("Sa_Cod"))
                If sa > 0 Then centri.Add(Tuple.Create(p.Trim(), sa))
            Next
        Else
            Dim sqlImprese = AgronicaCoreVisibilitaStd.VisibilitaQueryBuilder.BuildQueryImprese(descrizione2, AgronicaCoreVisibilitaStd.ModalitaQuery.UvaLean)
            Dim sqlCentri = AgronicaCoreVisibilitaStd.VisibilitaQueryBuilder.BuildQueryCentri(descrizione2, AgronicaCoreVisibilitaStd.ModalitaQuery.UvaLean)
            Dim parsImprese = BuildParametriVisibilita_Std(objParametri_server)

            Dim dtImprese = EseguiQuery_Lettura(objParametri_server, sqlImprese, parsImprese, "CalcolaGerarchia_Std.Imprese")
            ReadPive_Std(dtImprese, pive)

            Dim parsCentri = BuildParametriVisibilita_Std(objParametri_server)
            Dim dtCentri = EseguiQuery_Lettura(objParametri_server, sqlCentri, parsCentri, "CalcolaGerarchia_Std.Centri")
            ReadCentri_Std(dtCentri, centri)
        End If
    End Sub

    Private Function BuildParametriVisibilita_Std(objParametri_server As AgronicaCoreParametri) As Dictionary(Of String, Object)
        Return New Dictionary(Of String, Object) From {
            {"@pivaSuperUser", objParametri_server.PivaSuperUser},
            {"@finestraInizio", objParametri_server.FinestraTemporaleInizio},
            {"@finestraFine", objParametri_server.FinestraTemporaleFine}
        }
    End Function

    Private Function LeggiImpreseCentriDaPratiche(
        utente_Username As String, objParametri_Utenti As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri
    ) As HashSet(Of Tuple(Of String, Integer))
        Dim dal As New AgronicaCoreUtentiDAL.Utenti_Profili_Pratiche
        Return dal.LeggiImpreseCentriDaPratiche(utente_Username, objParametri_Utenti, objParametri_Server).
            AsEnumerable().
            Select(Function(row) New Tuple(Of String, Integer)(row.Field(Of String)("piva"), row.Field(Of Integer)("Sa_Cod"))).
            ToHashSet()
    End Function

    'Private Function LeggiPraticheSelezionate_Std(
    '    utente_Username As String, objParametri_Utenti As AgronicaCoreParametri
    ') As List(Of AgronicaCoreVisibilitaStd.Models.PraticaProfilo)
    '    Dim risp As New List(Of AgronicaCoreVisibilitaStd.Models.PraticaProfilo)
    '    Dim dal As New AgronicaCoreUtentiDAL.Utenti_Profili_Pratiche
    '    Dim dt = dal.Leggi(utente_Username, objParametri_Utenti)
    '    If dt Is Nothing Then Return risp
    '    For Each row As DataRow In dt.Rows
    '        If row("Servizio_Cod") Is DBNull.Value Then Continue For
    '        Dim servCod = Convert.ToInt32(row("Servizio_Cod"))
    '        Dim consideraVT As Boolean = False
    '        If row("Considera_Validita_Temporale") IsNot DBNull.Value Then
    '            consideraVT = Convert.ToBoolean(row("Considera_Validita_Temporale"))
    '        End If
    '        risp.Add(New AgronicaCoreVisibilitaStd.Models.PraticaProfilo(servCod, consideraVT))
    '    Next
    '    Return risp
    'End Function

    Private Function CalcolaPivePratiche_Std(
        pratiche As List(Of AgronicaCoreVisibilitaStd.Models.PraticaProfilo),
        objParametri_server As AgronicaCoreParametri
    ) As HashSet(Of String)
        Dim risp As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim build = AgronicaCoreVisibilitaStd.VisibilitaQueryBuilder.BuildQueryPratiche(pratiche)
        If build Is Nothing Then Return risp

        build.Parametri("@pivaSuperUser") = objParametri_server.PivaSuperUser

        Dim dt = EseguiQuery_Lettura(objParametri_server, build.Sql, build.Parametri, "CalcolaPivePratiche_Std")
        ReadPive_Std(dt, risp)
        Return risp
    End Function

    'Private Function LeggiCentriPerPive_Std(
    '    pive As List(Of String),
    '    objParametri_server As AgronicaCoreParametri
    ') As HashSet(Of Tuple(Of String, Integer))
    '    Dim risp As New HashSet(Of Tuple(Of String, Integer))()
    '    Dim lista = pive.Where(Function(p) Not String.IsNullOrWhiteSpace(p)).
    '                    Select(Function(p) p.Trim()).
    '                    Distinct(StringComparer.OrdinalIgnoreCase).
    '                    ToList()
    '    If lista.Count = 0 Then Return risp
    '    Dim pars As New Dictionary(Of String, Object) From {
    '        {"@finestraInizio", objParametri_server.FinestraTemporaleInizio},
    '        {"@finestraFine", objParametri_server.FinestraTemporaleFine}
    '    }
    '    Dim placeholders As New List(Of String)
    '    For i = 0 To lista.Count - 1
    '        Dim name = "@pivaCentri_" & i
    '        placeholders.Add(name)
    '        pars(name) = lista(i)
    '    Next
    '    Dim sql As String =
    '        " SELECT Centri_Aziendali.PIVA, Centri_Aziendali.sa_cod " &
    '        " FROM   Centri_Aziendali (NOLOCK) " &
    '        " WHERE  Centri_Aziendali.PIVA IN (" & String.Join(", ", placeholders) & ") " &
    '        "   AND  Centri_Aziendali.Validita_Inizio <= @finestraFine " &
    '        "   AND  Centri_Aziendali.Validita_Fine   >= @finestraInizio "
    '    Dim dt = EseguiQuery_Lettura(objParametri_server, sql, pars, "LeggiCentriPerPive_Std")
    '    ReadCentri_Std(dt, risp)
    '    Return risp
    'End Function

    Private Function BuildSchemaUVA_Std() As DataTable
        Dim dt As New DataTable
        dt.Columns.Add(New DataColumn("PivaSuperUser", GetType(String)))
        dt.Columns.Add(New DataColumn("Username", GetType(String)))
        dt.Columns.Add(New DataColumn("Entita_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Id_Reg", GetType(Integer)))
        Return dt
    End Function

    Private Function BuildDataTableUVAImprese_Std(
        pivaSuperUser As String, username As String, pive As HashSet(Of String)
    ) As DataTable
        Dim dt = BuildSchemaUVA_Std()
        For Each p In pive
            Dim row = dt.NewRow()
            row("PivaSuperUser") = pivaSuperUser
            row("Username") = username
            row("Entita_Cod") = CInt(enum_TipoEntita.Impresa)
            row("Piva") = p
            row("Sa_Cod") = 0
            row("Appezza") = 0
            row("Id_Reg") = 0
            dt.Rows.Add(row)
        Next
        Return dt
    End Function

    Private Function BuildDataTableUVACentri_Std(
        pivaSuperUser As String, username As String, centri As HashSet(Of Tuple(Of String, Integer))
    ) As DataTable
        Dim dt = BuildSchemaUVA_Std()
        For Each c In centri
            Dim row = dt.NewRow()
            row("PivaSuperUser") = pivaSuperUser
            row("Username") = username
            row("Entita_Cod") = CInt(enum_TipoEntita.Centro)
            row("Piva") = c.Item1
            row("Sa_Cod") = c.Item2
            row("Appezza") = 0
            row("Id_Reg") = 0
            dt.Rows.Add(row)
        Next
        Return dt
    End Function

    Private Sub ReadPive_Std(dt As DataTable, ByRef sink As HashSet(Of String))
        If dt Is Nothing OrElse Not dt.Columns.Contains("Piva") Then Return
        For Each row As DataRow In dt.Rows
            If row("Piva") Is DBNull.Value Then Continue For
            Dim p = Convert.ToString(row("Piva"))
            If Not String.IsNullOrWhiteSpace(p) Then sink.Add(p.Trim())
        Next
    End Sub

    Private Sub ReadCentri_Std(dt As DataTable, ByRef sink As HashSet(Of Tuple(Of String, Integer)))
        If dt Is Nothing OrElse Not dt.Columns.Contains("Piva") OrElse Not dt.Columns.Contains("Sa_Cod") Then Return
        For Each row As DataRow In dt.Rows
            If row("Piva") Is DBNull.Value OrElse row("Sa_Cod") Is DBNull.Value Then Continue For
            Dim p = Convert.ToString(row("Piva"))
            If String.IsNullOrWhiteSpace(p) Then Continue For
            sink.Add(Tuple.Create(p.Trim(), Convert.ToInt32(row("Sa_Cod"))))
        Next
    End Sub
#End Region

#Region "Password e Accesso"
    Private Function createDTUVA()
        Dim dt As New DataTable
        dt.Columns.Add(New DataColumn("PivaSuperUser", GetType(String)))
        dt.Columns.Add(New DataColumn("Username", GetType(String)))
        dt.Columns.Add(New DataColumn("Entita_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Id_Reg", GetType(Integer)))
        Return dt
    End Function

    Private Function getConnectionStringFromOleToSql(ByVal sConnessioneOle As String) As String
        Dim separator(1) As Char
        separator(0) = ";"c
        separator(1) = "="c

        Dim rval As String = ""

        Dim DaTenere As String = "server,data source,initial catalog,user id,password"

        Dim arrayFromString() As String = sConnessioneOle.Split(separator)
        For i As Integer = 0 To arrayFromString.Length - 2 Step 2
            If DaTenere.IndexOf(Trim(arrayFromString(i)).ToLower, 0) > -1 Then
                rval &= arrayFromString(i) & "=" & arrayFromString(i + 1) & ";"
            End If
        Next

        Return rval
    End Function

    Public Shared Function MessaggioRequisitiPassword(Optional ByVal minLength As Integer = 8,
                                                      Optional ByVal numLower As Integer = 1,
                                                      Optional ByVal numUpper As Integer = 1,
                                                      Optional ByVal numNumbers As Integer = 1,
                                                      Optional ByVal numSpecial As Integer = 1,
                                                      Optional ByVal gestioneHashAbilitata As Boolean? = Nothing,
                                                      Optional ByRef objParametri_Server As AgronicaCoreParametri = Nothing) As String

        Dim mes As String = Gias.RequisitiPassword

        Dim listaReq As New List(Of String)()
        listaReq.Add(String.Format(Gias.RequisitoPasswordLunghezzaMinima, minLength))
        listaReq.Add(String.Format(Gias.RequisitoPasswordLettereMinuscole, numLower))
        listaReq.Add(String.Format(Gias.RequisitoPasswordLettereMaiuscole, numUpper))
        listaReq.Add(String.Format(Gias.RequisitoPasswordNumeri, numNumbers))
        listaReq.Add(String.Format(Gias.RequisitoPasswordCaratteriSpeciali, numSpecial))

        mes &= " " & String.Join(", ", listaReq)

        If Not gestioneHashAbilitata.HasValue Then

            If objParametri_Server IsNot Nothing Then
                Dim handleConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim dtConfigSiti As DataTable = handleConfigSiti.Leggi(0, "AbilitaHashPassword", "", "", objParametri_Server)

                If dtConfigSiti.Rows.Count > 0 Then
                    gestioneHashAbilitata = CBool(dtConfigSiti.Rows(0)("Valore"))
                Else
                    gestioneHashAbilitata = False
                End If
            Else
                gestioneHashAbilitata = False
            End If

        End If

        If gestioneHashAbilitata = False Then
            mes &= ". " & String.Format(Gias.RequisitoPasswordCaratteriNonSupportati, PASSWORD_CARATTERI_SPECIALI_AMMESSI)
        End If

        Return mes
    End Function

    ''' <summary>Determines if a password is sufficiently complex.</summary>
    ''' <param name="pwd">Password to validate</param>
    ''' <param name="minLength">Minimum number of password characters.</param>
    ''' <param name="numUpper">Minimum number of uppercase characters.</param>    
    ''' <param name="numNumbers">Minimum number of numeric characters.</param>
    ''' <param name="numSpecial">Minimum number of special characters.</param>
    ''' <returns>True if the password is sufficiently complex.</returns>
    Public Shared Function ValidaComplessitaPassword(ByVal pwd As String,
        Optional ByVal minLength As Integer = 8,
        Optional ByVal numLower As Integer = 1,
        Optional ByVal numUpper As Integer = 1,
        Optional ByVal numNumbers As Integer = 1,
        Optional ByVal numSpecial As Integer = 1,
        Optional ByVal gestioneHashAbilitata As Boolean = False) As Boolean

        ' Replace [A-Z] with \p{Lu}, to allow for Unicode uppercase letters.
        Dim lower As New System.Text.RegularExpressions.Regex("[a-z]", RegexOptions.None, TimeSpan.FromSeconds(3))
        Dim upper As New System.Text.RegularExpressions.Regex("[A-Z]", RegexOptions.None, TimeSpan.FromSeconds(3))
        Dim number As New System.Text.RegularExpressions.Regex("[0-9]", RegexOptions.None, TimeSpan.FromSeconds(3))
        ' Special is "none of the above".
        Dim special As New System.Text.RegularExpressions.Regex("[^a-zA-Z0-9]", RegexOptions.None, TimeSpan.FromSeconds(3))

        ' Check the length.
        If Len(pwd) < minLength Then Return False
        ' Check for minimum number of occurrences.
        If lower.Matches(pwd).Count < numLower Then Return False
        If upper.Matches(pwd).Count < numUpper Then Return False
        If number.Matches(pwd).Count < numNumbers Then Return False
        If special.Matches(pwd).Count < numSpecial Then Return False

        If gestioneHashAbilitata = False AndAlso Regex.IsMatch(pwd, EXPREG_PASSWORD, RegexOptions.None, TimeSpan.FromSeconds(3)) = False Then Return False

        ' Passed all checks.
        Return True
    End Function

    ''' <summary>
    ''' Genera una nuova password utilizzando caratteri printabili della codifica ASCII
    ''' </summary>
    ''' <param name="minLength"></param>
    ''' <param name="numLower"></param>
    ''' <param name="numUpper"></param>
    ''' <param name="numNumbers"></param>
    ''' <param name="numSpecial"></param>
    ''' <returns></returns>
    Public Shared Function GeneraPasswordRequisiti(Optional ByVal minLength As Integer = 8,
        Optional ByVal numLower As Integer = 1,
        Optional ByVal numUpper As Integer = 1,
        Optional ByVal numNumbers As Integer = 1,
        Optional ByVal numSpecial As Integer = 1) As String

        Dim nuovaPassword As String

        Dim rand As New Random()

        Do
            nuovaPassword = ""

            Do
                Dim charCode = rand.Next(33, 127)
                Dim newChar = Chr(charCode)

                If Regex.IsMatch(newChar, EXPREG_PASSWORD, RegexOptions.None, TimeSpan.FromSeconds(3)) Then 'Ammetto solo un sotto-insieme di questi caratteri
                    nuovaPassword &= newChar
                End If

            Loop While nuovaPassword.Length < minLength

            'Ciclo esterno perché devo capire se la password generata rispetta i requisiti nella sua interezza
        Loop While ValidaComplessitaPassword(nuovaPassword, minLength, numLower, numUpper, numNumbers, numSpecial, False) = False

        Return nuovaPassword

    End Function

    Public Function NumeroTentativiAccessoScrivi(utente As String, NumeroTentativiAccesso As Integer, objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim xscriviTentativia As New AgronicaCoreUtentiDAL.Utenti_Write
        Return xscriviTentativia.ModificaLockTentativi(utente, NumeroTentativiAccesso, objParametri_Utenti)

    End Function

    Public Function NumeroTentativiAccessoLeggi(utente As String, objParametri_Utenti As AgronicaCoreParametri) As Integer

        Dim NumeroTentativiAccesso As Integer = 0

        Dim xLeggiNumeroTentativi As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim dtLeggi As DataTable = xLeggiNumeroTentativi.LeggiLockTentativi(utente, "", "", objParametri_Utenti)

        If dtLeggi.Rows.Count > 0 Then

            Dim tS As String = dtLeggi.Rows(0)("LockTentativi").ToString
            If Not String.IsNullOrEmpty(tS) Then
                NumeroTentativiAccesso = CInt(tS)
            End If

        End If


        Return NumeroTentativiAccesso

    End Function
#End Region

    Public Function Aggiorna_Licenze(
        Operazione As Integer,
        Username As String,
        Progressivo_Gias As Integer,
        CD_Key As String,
        GiasOnline_Key As String,
        Durata As Integer,
        Versione As Integer,
        Aziende As Integer,
        Sup As Integer,
        ByRef objParametri As AgronicaCoreParametri
    ) As Boolean
        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreContabUtentiBIZ.Aggiorna_Licenze()"
        Dim Classe As Integer = 2
        Dim Moduli As Integer = 31
        Dim DataAttivazione As Date = CDate(Now)
        Dim bOk As Boolean = False
        Try
            Dim Utenti_W As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPro_W
            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                Select Case Operazione
                    Case 1 'Scrittura
                        bOk = Utenti_W.Scrivi(Username, Progressivo_Gias, CD_Key, DataAttivazione, Durata, Aziende, Classe, Moduli, "agronica", 0, CDate(Now), objParametri, GiasOnline_Key)

                    Case 2 'Modifica
                        bOk = Utenti_W.Modifica(Username, CD_Key, DataAttivazione, Durata, Aziende, Classe, Moduli, "agronica", 0, objParametri, GiasOnline_Key)

                End Select

                If bOk Then
                    ' COMIT Effettivo
                    scope.Complete()
                End If
            End Using
        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If
        Return bOk
    End Function

    Public Function Aggiorna_Licenze(
        righeInseriteGrid_Licenze As String,
        righeModificateGrid_Licenze As String,
        righeCancellateGrid_Licenze As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As Integer
        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreContabUtentiBIZ.Aggiorna_Licenze()"
        Dim Dummy As Integer
        Dim righeArray_Modificate As JArray
        Try
            Dim Utenti_W As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPro_W
            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                If righeModificateGrid_Licenze <> "" Then
                    righeArray_Modificate = JArray.Parse(righeModificateGrid_Licenze)

                    For Each obj As JObject In righeArray_Modificate
                        Dummy = Utenti_W.Aggiorna_Licenze(obj("ProgressivoGIAS"), obj("UserName"), obj("GiasOnline_Key"), obj("CD_Key"), objParametri)
                    Next

                    If Dummy = 0 Then
                        ' COMIT Effettivo
                        scope.Complete()
                    End If
                End If
            End Using
        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If
        Return 1
    End Function

    Public Function Aggiorna_Superuser(
        Operazione As Integer,
        username As String,
        password As String,
        tipo_utente As Integer,
        piva As String,
        ragione_sociale As String,
        cognome As String,
        nome As String,
        cod_fisc As String,
        via As String,
        numero As String,
        cap As String,
        citta As String,
        provincia As String,
        Telefono As String,
        Fax As String,
        Email As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As Integer
        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreContabUtentiBIZ.Aggiorna_Superuser()"
        Dim Dummy As Integer = -1
        Dim bOk As Boolean = False
        Try
            Dim Utenti_W As New AgronicaCoreUtentiDAL.Utenti_Write
            Dim Utenti_Dettagli_W As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W

            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                Select Case Operazione
                    Case 1 'SALVATAGGIO
                        Dummy = Utenti_W.Scrivi_Superuser(username, password, objParametri)
                        If Dummy = 0 Then
                            bOk = Utenti_Dettagli_W.Scrivi(
                                username, cognome, nome, via, numero, citta, provincia, cap, Telefono, Fax, Email, piva,
                                cod_fisc, ragione_sociale, tipo_utente, 0, "agronica", objParametri
                            )
                        End If

                    Case 2 'MODIFICA
                        Dummy = Utenti_W.Modifica_Superuser(username, password, objParametri)
                        If Dummy = 0 Then
                            bOk = Utenti_Dettagli_W.Modifica(
                                username, cognome, nome, via, numero, citta, provincia, cap, Telefono, Fax, Email, piva,
                                cod_fisc, ragione_sociale, tipo_utente, 0, "agronica", objParametri
                            )
                        End If
                End Select

                If bOk Then
                    ' COMIT Effettivo
                    scope.Complete()
                    Dummy = 1
                Else
                    Dummy = -1
                End If
            End Using

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Dummy = -1
        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If
        Return Dummy
    End Function

    Public Function Aggiorna_Visibilita_Area(
        righeInserite As String,
        righeModificate As String,
        righeCancellate As String,
        ByRef objParametriUtenti As AgronicaCoreParametri
    ) As Boolean
        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreUtentiBIZ.UMA_Richieste.Aggiorna_Visibilita_Area()"
        Dim log As New AgronicaCoreDataProvider.LogProvider
        Dim bOk As Boolean = False
        Dim Jinsert As JArray
        Dim Jdelete As JArray
        Dim QArrayToInsert As New ArrayList
        Dim QArrayToUpdate As New ArrayList
        Dim QArrayToDelete As New ArrayList
        Dim Risultato As Integer = 0
        Dim visibilitaDAL = New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
        Dim inOK As Boolean = False 'per controllare se ci sono stati dei nuovi record inseriti
        Dim r As String = ""

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitato_VisibilitaCompleta = objPermessi.Controlla_Permessi_Utente(
            objParametriUtenti.UtenteUsername, enum_Id_Servizio.GiasOnline, enum_Security_Attivita.Visibilita_Aziende_UMA_GestioneVisibilitaCompleta,
            enum_Security_Operazione.Modifica, Date.Now, "", objParametriUtenti
        )
        Try
            Jinsert = JArray.Parse(righeInserite)
            If Jinsert.Count Then
                inOK = True
            End If
            If inOK Then
                'creo i record da inserire
                For Each objRiga As JObject In Jinsert
                    If CStr(objRiga.Item("Piva_Azienda")) <> "" Then
                        r = String.Format(
                            "INSERT INTO Utenti_Visibilita (Gruppo, UserName, Area, Piva_Azienda, Username_Creazione, Username_Modifica) VALUES ({0}, '{1}', 0, '{2}', '{3}', '{3}')",
                            Agro_SQL_SaveNum(CInt(objRiga.Item("Gruppo_Cod")).ToString),
                            Agro_SQL_SaveText(CStr(objRiga.Item("UserName"))),
                            Agro_SQL_SaveText(CStr(objRiga.Item("Piva_Azienda"))),
                            objParametriUtenti.UsernameOperazione
                        )

                    ElseIf UtenteAbilitato_VisibilitaCompleta Then
                        r = String.Format(
                            "INSERT INTO Utenti_Visibilita (Gruppo, UserName, Area, Piva_Azienda, Username_Creazione, Username_Modifica, Visibilita_Completa) VALUES ({0}, '{1}', 0, '{2}', '{3}', '{3}', 1)",
                            Agro_SQL_SaveNum(CInt(objRiga.Item("Gruppo_Cod")).ToString),
                            Agro_SQL_SaveText(CStr(objRiga.Item("UserName"))),
                            Agro_SQL_SaveText(CStr(objRiga.Item("Piva_Azienda"))),
                            objParametriUtenti.UsernameOperazione
                        )

                    Else
                        Throw New Exception("Visibilità Completa non gestibile dall'utente")

                    End If

                    QArrayToInsert.Add(r)
                Next
            End If

            If righeCancellate <> "" And righeCancellate <> "[]" Then
                Jdelete = JArray.Parse(righeCancellate)
                For Each objRiga As JObject In Jdelete
                    r = "DELETE FROM Utenti_Visibilita WHERE Gruppo = " + CInt(IIf(IsNothing(objRiga.Item("Gruppo_Cod")), 0, objRiga.Item("Gruppo_Cod"))).ToString + " AND UserName like '" + Agro_SQL_SaveText(CStr(objRiga.Item("UserName"))) + "' AND Piva_Azienda like '" + Agro_SQL_SaveText(CStr(objRiga.Item("Piva_Azienda"))) + "' "
                    QArrayToDelete.Add(r)
                Next

            End If

            Risultato = visibilitaDAL.Aggiorna_Visibilita(QArrayToInsert, QArrayToDelete, objParametriUtenti)

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            log.Scrivi_LOG(objParametriUtenti, NomeRoutine, MessaggioErrore)
            Throw New Exception(MessaggioErrore)

        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If
        Return Risultato
    End Function

#Region "Gestione utenti"
    Public Function CaricaUtentiEntryPoint(ByVal Jfilters As JArray, params As ObjParams) As DataTable
        Dim isFirstTime = Jfilters.Where(Function(f) f("field") = "firstTime").
            Select(Function(f) String.IsNullOrEmpty(f("value")) OrElse CBool(f("value"))).
            DefaultIfEmpty(False).First
        Dim userFiltersUtility = New UtentiFiltriUtility
        Dim FiltroUtenti = userFiltersUtility.GetFilterStrFromJArray(Jfilters)
        Dim dt As DataTable = Nothing

        If isFirstTime AndAlso IsSmallOrMediumBusiness(params.ObjParametri_Server, params.ObjParametri_Utenti) Then
            dt = Carica_Utenti(String.Empty, params)
        Else
            Dim ObjUtentiDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
            Dim Dt_Utenti As DataTable = ObjUtentiDettagli.LeggiCompleta(
                enum_Id_Servizio.GiasOnline, Id_Attivita:=-1,
                enumSelezioneVariabile.Selezione_JoinDescrizioni,
                FiltroUtenti, "", params.ObjParametri_Utenti
            )
            If Dt_Utenti.Rows.Count > 0 Then
                Dt_Utenti = Dt_Utenti.Select.AsParallel.
                    GroupBy(Function(row) row("UserName")).
                    Select(Function(duplicated) duplicated.First).
                    OrderBy(Function(row) row("UserName")).
                    CopyToDataTable
            End If
            dt = LoadUserData(Dt_Utenti, params)
        End If

        If dt.Columns.Contains("Password") Then
            dt.Columns.Remove("Password")
        End If
        dt.Columns.Add("Password")

        Return dt
    End Function

    Public Function GetUsersCount(params As ObjParams) As Integer
        Dim leggiUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
        Return leggiUtenti.ContaUtenti(String.Empty, params.ObjParametri_Utenti)
    End Function

    Public Function CaricaUtentiEntryPoint(ByVal UtentiVisibili As String, params As ObjParams)
        Dim smallMediumBusiness = 200
        Dim userCount = GetUsersCount(params)
        If userCount <= smallMediumBusiness Then
            Return Carica_Utenti(String.Empty, params)
        Else
            Return Carica_Utenti(UtentiVisibili, params)
        End If

    End Function

    Private Function IsSmallOrMediumBusiness(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim configSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim chiaveStr = configSiti.Leggi_Valore(
            Sito_Cod:=0, Chiave:="AgroProfilazione_MaxUtentiCaricatiDefault",
            xFiltroAggiuntivo:=String.Empty, xOrderBy:=String.Empty, objParametri_Server
        )
        Dim smallMediumBusiness = If(String.IsNullOrWhiteSpace(chiaveStr), 200, CInt(chiaveStr))
        Dim userCount = GetUsersCount(New ObjParams With {.ObjParametri_Utenti = objParametri_Utenti})
        Return userCount <= smallMediumBusiness
    End Function

    ''' <summary>
    ''' Carica i dettagli riguardanti GDPR (accettazione privacy), finestra temporale,
    ''' servizi di stato (per Coldiretti), accessi, gruppi utenti, accesso spid.
    ''' </summary>
    ''' <param name="Dt_Utenti">DataTable con gli utenti da considerare</param>
    Private Function LoadUserData(ByRef Dt_Utenti As DataTable, params As ObjParams) As DataTable
        If IsNothing(Dt_Utenti) OrElse Dt_Utenti.Rows.Count = 0 Then
            Return Dt_Utenti
        End If

        Dim sql2017 = False
        Dim major = VersioneSqlServer_Major(params.ObjParametri_Utenti)

        If major >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            sql2017 = True ' Se ho la versione 2017 o superiore i gruppi sono già letti nella query
        End If

        Dim finestraTempDic = LeggiDatiFinestraTemporale(params.ObjParametri_Server)
        '-- Legge i servizi attivi per Coldiretti
        Dim descrizioneServizioStatoQDCArray = LeggiDatiServizioStato(params.ObjParametri_Server)
        '-- Legge dati per accessi
        Dim objAWS_Log = New AgronicaCoreUtentiBIZ.AWS_Log_R
        Dim conteggioAbilitato As Boolean = objAWS_Log.IsConteggioAccessiAbilitato(params.ObjParametri_Server)
        '-- Legge dati per gruppi utenti
        Dim gr As New Dictionary(Of String, DataRow())
        If Not sql2017 Then
            Dim objGU As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
            gr = objGU.LeggiUtenti(
                "", 0, " Utenti_xGruppi_Utente.Gruppi_Utente_cod IS NOT NULL ", "", params.ObjParametri_Utenti
            ).Select.GroupBy(Function(row) CStr(row("UserName"))).
            ToDictionary(Function(g) g.Key, Function(g) g.ToArray)
            Dt_Utenti.Columns.Add(New DataColumn("GruppiCod", GetType(String)))
            Dt_Utenti.Columns.Add(New DataColumn("GruppiDes", GetType(String)))
        End If
        'Leggo i dati per la gestione dell'accesso con SPID
        Dim accessiSpid As New Dictionary(Of String, UtenteAccessoSPID)
        If gestioneSpidAbilitata(params) Then
            accessiSpid = recuperaDatiAccessoSpid(String.Empty, params.ObjParametri_Utenti)
            Dt_Utenti.Columns.Add(New DataColumn("Flag_Accesso_SPID", GetType(Boolean)))
            Dt_Utenti.Columns.Add(New DataColumn("CF_SPID", GetType(String)))
            Dt_Utenti.Columns.Add(New DataColumn("CF_SPID_Corrente", GetType(String)))
        End If

        addUserColumns(Dt_Utenti)

        For i As Integer = 0 To Dt_Utenti.Rows.Count - 1

            If Dt_Utenti.Rows(i).Item("Flag_Azienda_Persona") = 1 Then
                Dt_Utenti.Rows(i).Item("Azienda_Persona") = "I"
                Dt_Utenti.Rows(i).Item("Dettagli") = Dt_Utenti.Rows(i).Item("Rag_Soc") & ""
            Else
                Dt_Utenti.Rows(i).Item("Azienda_Persona") = "P"
                Dt_Utenti.Rows(i).Item("Dettagli") = Dt_Utenti.Rows(i).Item("Cognome") & " " & Dt_Utenti.Rows(i).Item("Nome")
            End If

            If IsDate(Dt_Utenti.Rows(i).Item("Validita_Inizio")) AndAlso CDate(Dt_Utenti.Rows(i).Item("Validita_Inizio")) <> AGRODATAINIZIO Then
                Dt_Utenti.Rows(i).Item("Data_Inizio") = CDate(Dt_Utenti.Rows(i).Item("Validita_Inizio")).ToShortDateString
            Else
                Dt_Utenti.Rows(i).Item("Data_Inizio") = "..."
            End If
            If IsDate(Dt_Utenti.Rows(i).Item("Validita_Fine")) AndAlso CDate(Dt_Utenti.Rows(i).Item("Validita_Fine")) <> AGRODATAFINE Then
                Dt_Utenti.Rows(i).Item("Data_Fine") = CDate(Dt_Utenti.Rows(i).Item("Validita_Fine")).ToShortDateString
            Else
                Dt_Utenti.Rows(i).Item("Data_Fine") = "..."
            End If

            Dt_Utenti.Rows(i).Item("Attivo") = If(CDate(Dt_Utenti.Rows(i).Item("Validita_Fine")) >= Date.Today, 1, 0)
            Dt_Utenti.Rows(i).Item("kendoKey") = i

            'Servizi attivi
            Dim serviziAttivi As String = ""
            Dim username As String = Dt_Utenti.Rows(i).Item("UserName")
            If Not IsNothing(descrizioneServizioStatoQDCArray) AndAlso descrizioneServizioStatoQDCArray.Count > 0 Then
                Dim serviziAttiviPerUtente As List(Of String) = (From a As JObject In descrizioneServizioStatoQDCArray
                                                                 Where CStr(a("Utente")).ToUpper() = username.ToUpper()
                                                                 Select CStr(a("ServizioDes")) & ": " & CStr(a("StatoDes"))).Distinct().ToList()
                serviziAttivi = String.Join(" - ", serviziAttiviPerUtente)
            End If
            Dt_Utenti.Rows(i).Item("serviziAttivi") = serviziAttivi

            '-- Dati accessi
            If Not conteggioAbilitato Then
                Dt_Utenti.Rows(i).Item("NumeroAccessi") = 0
            End If

            '-- gruppi utente
            If Not sql2017 Then
                Dim strGruppiCod = ""
                Dim strGruppiDes = ""
                Dim DrGruppi = If(gr.ContainsKey(username.ToUpper()), gr(username.ToUpper()), Nothing)
                If DrGruppi IsNot Nothing AndAlso DrGruppi.Any Then
                    strGruppiCod = DrGruppi.Select(Function(dr) CStr(dr.Item("Gruppi_Utente_cod"))).
                        Aggregate(Function(acc, cod) acc & "|" & cod)
                    strGruppiDes = DrGruppi.Select(Function(dr) CStr(dr.Item("Gruppi_Utente_des"))).
                        Aggregate(Function(acc, cod) acc & "|" & cod)
                End If
                Dt_Utenti.Rows(i).Item("GruppiCod") = strGruppiCod
                Dt_Utenti.Rows(i).Item("GruppiDes") = strGruppiDes
            End If

            If accessiSpid.ContainsKey(username.ToUpper()) Then
                Dt_Utenti.Rows(i).Item("Flag_Accesso_SPID") = accessiSpid(username.ToUpper()).ObbligaLoginSPID
                Dt_Utenti.Rows(i).Item("CF_SPID") = accessiSpid(username.ToUpper()).CfLogin
                Dt_Utenti.Rows(i).Item("CF_SPID_Corrente") = accessiSpid(username.ToUpper()).CfLogin
            End If

            If finestraTempDic.ContainsKey(username.ToUpper()) Then
                Dt_Utenti.Rows(i).Item("FinestraTemporaleInizio") = finestraTempDic(username.ToUpper()).FinestraTemporale.inizio
                Dt_Utenti.Rows(i).Item("FinestraTemporaleFine") = finestraTempDic(username.ToUpper()).FinestraTemporale.fine
            Else
                Dt_Utenti.Rows(i).Item("FinestraTemporaleInizio") = AGRODATAINIZIO
                Dt_Utenti.Rows(i).Item("FinestraTemporaleFine") = AGRODATAFINE
            End If

        Next
        Return Dt_Utenti
    End Function

    Public Function Carica_Utenti(ByVal UtentiVisibili As String, ASG_IdServizio As Integer,
        objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As DataTable

        Dim params As New ObjParams With {
            .ObjParametri_Utenti = objParametri_Utenti,
            .ObjParametri_Server = objParametri_Server,
            .ObjParametri_SuperServer = Nothing 'Usato solo per verificare l'abilitazione dello spid
        }
        Return Carica_Utenti(UtentiVisibili, params)
    End Function

    ''' <param name="UtentiVisibili">Lista di utenti da leggere. Gli username devono essere separati da virgola.</param>
    Public Function Carica_Utenti(ByVal UtentiVisibili As String, params As ObjParams) As DataTable
        Dim getStringOrDefault = Function(row As DataRow, field As String) If(IsDBNull(row(field)), String.Empty, CStr(row(field)))
        Dim getDateOr = Function(row As DataRow, field As String, defaultValue As Date) If(IsDBNull(row(field)), defaultValue, CDate(row(field)))
        Dim FiltroUtenti As String = ""
        Dim sql2017 = False

        Dim major = VersioneSqlServer_Major(params.ObjParametri_Utenti)
        If major >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            sql2017 = True ' Se ho la versione 2017 o superiore i gruppi sono già letti nella query
        End If

        'Check username format for SQL
        If Not String.IsNullOrWhiteSpace(UtentiVisibili) Then
            Dim formatForSQL = Function(u) If(u.StartsWith("'"), u, "'" & u & "'")
            If UtentiVisibili.Contains(",") Then
                UtentiVisibili = UtentiVisibili.Split(",").AsParallel.
                        Select(Function(str) str.Trim).
                        Where(Function(u) Not String.IsNullOrEmpty(u)).
                        Select(formatForSQL).
                        Aggregate(Function(strAcc, str) strAcc & "," & str)
            Else
                UtentiVisibili = formatForSQL(UtentiVisibili)
            End If
            FiltroUtenti = " Utenti_Dettagli.UserName IN (" & Agro_SQL_Save_Clausola_IN(UtentiVisibili, True) & ")"
        End If

        '-- Legge dati per GDPR
        Dim objGDPR = New AgronicaCoreUtentiBIZ.GDPR
        Dim gdprList As HashSet(Of String) = objGDPR.VerificaAccettazioneTutti(params.ObjParametri_Utenti).
            AsParallel.Select(Function(k) k.UserName).
            ToHashSet
        '-- Legge dati per accessi
        Dim objAWS_Log = New AgronicaCoreUtentiBIZ.AWS_Log_R
        Dim conteggioAbilitato As Boolean = objAWS_Log.IsConteggioAccessiAbilitato(params.ObjParametri_Server)
        Dim awsList As Dictionary(Of String, Utente_DettagliAWS) = objAWS_Log.UltimoAccessoTutti(params.ObjParametri_Utenti).
            ToDictionary(Function(aws) aws.UserName)
        '-- Legge dettagli utente
        Dim ObjUtentiDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim Dt_Utenti As DataTable = ObjUtentiDettagli.Leggi_anchePermessi(
            enum_Id_Servizio.GiasOnline, Id_Attivita:=-1,
            enumSelezioneVariabile.Selezione_JoinDescrizioni,
            FiltroUtenti, "", params.ObjParametri_Utenti)
        '-- Legge dati per gruppi utenti
        Dim gr As New Dictionary(Of String, DataRow())
        If Not sql2017 Then
            Dim objGU As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
            gr = objGU.LeggiUtenti(
                "", 0, " Utenti_xGruppi_Utente.Gruppi_Utente_cod IS NOT NULL ", "", params.ObjParametri_Utenti
            ).Select.GroupBy(Function(row) getStringOrDefault(row, "UserName")).
            ToDictionary(Function(g) g.Key, Function(g) g.ToArray)
            Dt_Utenti.Columns.Add(New DataColumn("GruppiCod", GetType(String)))
            Dt_Utenti.Columns.Add(New DataColumn("GruppiDes", GetType(String)))
        End If
        'Leggo i servizi attivi per Coldiretti
        Dim leggiServizioStatoQDC As New AgronicaCoreProfilazioneBIZ.Pratiche_R
        Dim xRisp As RispostaStandard = leggiServizioStatoQDC.leggiServiziStati(enum_WWorflow.Servizi_Agronica_2017, "", 0, "", params.ObjParametri_Server, Nothing)
        Dim descrizioneServizioStatoQDCArray As JArray = Nothing
        If xRisp.RispostaOK Then
            descrizioneServizioStatoQDCArray = JArray.Parse(xRisp.RispostaStringa)
        End If

        Dim accessiSpid As New Dictionary(Of String, UtenteAccessoSPID)
        'Leggo i dati per la gestione dell'accesso con SPID
        If gestioneSpidAbilitata(params) Then
            FiltroUtenti = If(String.IsNullOrEmpty(FiltroUtenti), FiltroUtenti, " UserName IN (" & UtentiVisibili & ")")
            accessiSpid = recuperaDatiAccessoSpid(FiltroUtenti, params.ObjParametri_Utenti)
            Dt_Utenti.Columns.Add(New DataColumn("Flag_Accesso_SPID", GetType(Boolean)))
            Dt_Utenti.Columns.Add(New DataColumn("CF_SPID", GetType(String)))
            Dt_Utenti.Columns.Add(New DataColumn("CF_SPID_Corrente", GetType(String)))
        End If

        addUserColumns(Dt_Utenti, True)

        If Dt_Utenti IsNot Nothing Then
            For i As Integer = 0 To Dt_Utenti.Rows.Count - 1
                Dim username = getStringOrDefault(Dt_Utenti.Rows(i), "UserName")

                If Dt_Utenti.Rows(i).Item("Flag_Azienda_Persona") = 1 Then
                    Dt_Utenti.Rows(i).Item("Azienda_Persona") = "I"
                    Dt_Utenti.Rows(i).Item("Dettagli") = getStringOrDefault(Dt_Utenti.Rows(i), "Rag_Soc")
                Else
                    Dt_Utenti.Rows(i).Item("Azienda_Persona") = "P"
                    Dt_Utenti.Rows(i).Item("Dettagli") = getStringOrDefault(Dt_Utenti.Rows(i), "Cognome") & " " & getStringOrDefault(Dt_Utenti.Rows(i), "Nome")
                End If

                If getDateOr(Dt_Utenti.Rows(i), "Validita_Inizio", AGRODATAINIZIO) <> AGRODATAINIZIO Then
                    Dt_Utenti.Rows(i).Item("Data_Inizio") = CDate(Dt_Utenti.Rows(i).Item("Validita_Inizio")).ToShortDateString
                Else
                    Dt_Utenti.Rows(i).Item("Data_Inizio") = "..."
                End If
                If getDateOr(Dt_Utenti.Rows(i), "Validita_Fine", AGRODATAFINE) <> AGRODATAFINE Then
                    Dt_Utenti.Rows(i).Item("Data_Fine") = CDate(Dt_Utenti.Rows(i).Item("Validita_Fine")).ToShortDateString
                Else
                    Dt_Utenti.Rows(i).Item("Data_Fine") = "..."
                End If

                Dt_Utenti.Rows(i).Item("Attivo") = If(getDateOr(Dt_Utenti.Rows(i), "Validita_Fine", AGRODATAFINE) >= Date.Today, 1, 0)
                Dt_Utenti.Rows(i).Item("kendoKey") = i

                'Servizi attivi
                Dim serviziAttivi As String = ""
                If Not IsNothing(descrizioneServizioStatoQDCArray) AndAlso descrizioneServizioStatoQDCArray.Count > 0 Then
                    Dim serviziAttiviPerUtente As List(Of String) = (From a As JObject In descrizioneServizioStatoQDCArray
                                                                     Where CStr(a("Utente")) = username
                                                                     Select CStr(a("ServizioDes")) & ": " & CStr(a("StatoDes"))).Distinct().ToList()
                    serviziAttivi = String.Join(" - ", serviziAttiviPerUtente)
                End If
                Dt_Utenti.Rows(i).Item("serviziAttivi") = serviziAttivi
                Dt_Utenti.Rows(i).Item("GDPR") = gdprList.Contains(username)

                '-- Dati accessi
                If awsList.ContainsKey(username) Then
                    Dt_Utenti.Rows(i).Item("UltimoAccesso") = awsList(username).UltimoAccesso
                    Dt_Utenti.Rows(i).Item("NumeroAccessi") = If(conteggioAbilitato, awsList(username).NumeroAccessi, 0)
                Else
                    Dt_Utenti.Rows(i).Item("NumeroAccessi") = 0
                End If

                '-- gruppi utente
                If Not sql2017 Then
                    Dim strGruppiCod = ""
                    Dim strGruppiDes = ""
                    Dim DrGruppi = If(gr.ContainsKey(username), gr(username), Nothing)
                    If DrGruppi IsNot Nothing AndAlso DrGruppi.Any Then
                        strGruppiCod = DrGruppi.Select(Function(dr) CStr(dr.Item("Gruppi_Utente_cod"))).
                            Aggregate(Function(acc, cod) acc & "|" & cod)
                        strGruppiDes = DrGruppi.Select(Function(dr) CStr(dr.Item("Gruppi_Utente_des"))).
                            Aggregate(Function(acc, cod) acc & "|" & cod)
                    End If
                    Dt_Utenti.Rows(i).Item("GruppiCod") = strGruppiCod
                    Dt_Utenti.Rows(i).Item("GruppiDes") = strGruppiDes
                End If

                If accessiSpid.ContainsKey(username) Then
                    Dt_Utenti.Rows(i).Item("Flag_Accesso_SPID") = accessiSpid(username).ObbligaLoginSPID
                    Dt_Utenti.Rows(i).Item("CF_SPID") = accessiSpid(username).CfLogin
                    Dt_Utenti.Rows(i).Item("CF_SPID_Corrente") = accessiSpid(username).CfLogin
                End If
            Next
        End If
        Return Dt_Utenti
    End Function

    'Aggiungo le colonne necessarie al gridview
    Private Sub addUserColumns(ByRef Dt_Utenti As DataTable, Optional addLoginInfoCols As Boolean = False)
        Dt_Utenti.Columns.Add(New DataColumn("kendoKey", GetType(String)))
        Dt_Utenti.Columns.Add(New DataColumn("Azienda_Persona", GetType(String)))
        Dt_Utenti.Columns.Add(New DataColumn("Dettagli", GetType(String)))
        Dt_Utenti.Columns.Add(New DataColumn("Data_Inizio", GetType(String)))
        Dt_Utenti.Columns.Add(New DataColumn("Data_Fine", GetType(String)))
        Dt_Utenti.Columns.Add(New DataColumn("Attivo", GetType(Integer)))
        Dt_Utenti.Columns.Add(New DataColumn("FinestraTemporaleInizio", GetType(Date)))
        Dt_Utenti.Columns.Add(New DataColumn("FinestraTemporaleFine", GetType(Date)))

        Dt_Utenti.Columns.Add(New DataColumn("ServiziAttivi", GetType(String)))
        If addLoginInfoCols Then
            Dt_Utenti.Columns.Add(New DataColumn("NumeroAccessi", GetType(String)))
            Dt_Utenti.Columns.Add(New DataColumn("UltimoAccesso", GetType(String)))
        End If
    End Sub

    Private Function gestioneSpidAbilitata(params As ObjParams) As Boolean
        Dim spidAbilitato As Boolean = False
        Dim configSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim valoreChiave As String = configSiti.Leggi_Valore(0, "paginaIndex_LoginSPID", "", "", params.ObjParametri_Server)
        If String.IsNullOrEmpty(valoreChiave) AndAlso params.ObjParametri_SuperServer IsNot Nothing Then
            valoreChiave = configSiti.Leggi_Valore(0, "paginaIndex_LoginSPID", "", "", params.ObjParametri_SuperServer)
        End If
        If Not String.IsNullOrEmpty(valoreChiave) Then
            Boolean.TryParse(valoreChiave, spidAbilitato)
        End If
        Return spidAbilitato
    End Function

    Private Function recuperaDatiAccessoSpid(FiltroUtenti As String, objParametri_Utenti As AgronicaCoreParametri) As Dictionary(Of String, UtenteAccessoSPID)
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim objCF As New AgronicaCoreUtentiDAL.UtentixCodFisc_R
        Dim accessiSpid As Dictionary(Of String, UtenteAccessoSPID)
        accessiSpid = objUtenti.Leggi(
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            FiltroUtenti, xOrderBy:=String.Empty, objParametri_Utenti
        ).Select.AsParallel.Select(Function(row) New UtenteAccessoSPID With {
            .ObbligaLoginSPID = Not IsDBNull(row("Flag_Accesso_SPID")) AndAlso Not row("Flag_Accesso_SPID") = 0,
            .UserName = CStr(row("UserName")).ToUpper()
        }).ToDictionary(Function(row) row.UserName.ToUpper())
        objCF.Leggi(UserName:=String.Empty, CodFisc:=String.Empty, FiltroUtenti, xOrderBy:=String.Empty, objParametri_Utenti).
            Select.AsParallel.
            Where(Function(row) accessiSpid.ContainsKey(row("UserName").ToUpper())).
            ForAll(Sub(row) accessiSpid(CStr(row("UserName")).ToUpper()).CfLogin = row("CodFisc"))
        Return accessiSpid
    End Function

    Private Function LeggiDatiFinestraTemporale(obj_Server As AgronicaCoreParametri) As Dictionary(Of String, IUtenteFinestraTemp)
        Dim objFinestraTemp_R As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim dict As New Dictionary(Of String, IUtenteFinestraTemp)
        Dim getDateOrElse = Function(row As DataRow, field As String, other As Date) If(IsDBNull(row(field)), other, row(field))
        objFinestraTemp_R.Leggi_Da_Gias_Server(xFiltroAggiuntivo:=String.Empty, xOrderBy:=String.Empty, obj_Server).
            Select.AsParallel.
            Select(Function(row) New UtenteFinestraTemp(
                CStr(row("USER")).ToUpper(),
                New IntervalloTemporale(
                    getDateOrElse(row, "FinestraTemp_Inizio", AGRODATAINIZIO),
                    getDateOrElse(row, "FinestraTemp_Fine", AGRODATAFINE)
                )
            )).ToList.ForEach(Sub(user) dict.Add(user.UserName.ToUpper(), user))
        Return dict
    End Function

    Private Function LeggiDatiGDPR(userTable As DataTable, objParametri_Utenti As AgronicaCoreParametri) As HashSet(Of String)
        Dim objGDPR = New AgronicaCoreUtentiBIZ.GDPR
        If userTable.Rows.Count > 10_000 Then
            Return objGDPR.VerificaAccettazioneTutti(objParametri_Utenti).
                AsParallel.Select(Function(k) k.UserName).
                ToHashSet
        Else
            Dim users = userTable.Select.AsParallel.
                Select(Function(row) New BaseUtente(CStr(row("UserName")))).ToArray
            Return objGDPR.VerificaAccettazione(users, objParametri_Utenti).
                AsParallel.Select(Function(k) k.UserName).
                ToHashSet
        End If
    End Function

    Private Function LeggiDatiServizioStato(objParametri_Server As AgronicaCoreParametri) As JArray
        Dim leggiServizioStatoQDC As New AgronicaCoreProfilazioneBIZ.Pratiche_R
        Dim xRisp As RispostaStandard = leggiServizioStatoQDC.leggiServiziStati(enum_WWorflow.Servizi_Agronica_2017, "", 0, "", objParametri_Server, Nothing)
        If xRisp.RispostaOK Then
            Return JArray.Parse(xRisp.RispostaStringa)
        Else
            Return Nothing
        End If
    End Function

    Public Function Carica_Utenti_Dati_Base(
        ASG_IdServizio As Integer,
        objParametri_Server As AgronicaCoreParametri,
        objParametri_Utenti As AgronicaCoreParametri
        ) As ListaUtenti

        Dim objUtentiDettagli As New Utenti_Dettagli_R
        Dim listaUtenti = New ListaUtenti()
        listaUtenti.ListaDatiBaseUtente = New List(Of DatiBaseUtente)

        Dim nessunFiltroIdAttivita As Integer = -1
        Dim getStringOr = Function(r As DataRow, field As String, defaultValue As String) If(Not IsDBNull(r.Item(field)), r.Item(field), defaultValue)
        Dim getIntegerOr = Function(r As DataRow, field As String, defaultValue As Integer) If(Not IsDBNull(r.Item(field)), r.Item(field), defaultValue)

        Dim Dt_Utenti As DataTable = objUtentiDettagli.Leggi_anchePermessi(
            ASG_IdServizio,
            nessunFiltroIdAttivita,
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            "",
            "",
            objParametri_Utenti)

        If Not IsNothing(Dt_Utenti) Then
            listaUtenti.ListaDatiBaseUtente = Dt_Utenti.Select.AsParallel.
                Select(Function(utente As DataRow)
                           Try
                               Dim datiBaseUtente = New DatiBaseUtente()
                               datiBaseUtente.UserName = getStringOr(utente, "UserName", "")
                               datiBaseUtente.Cognome = getStringOr(utente, "Cognome", "")
                               datiBaseUtente.Nome = getStringOr(utente, "Nome", "")
                               datiBaseUtente.RagioneSociale = getStringOr(utente, "Rag_Soc", "")
                               datiBaseUtente.Flag_Azienda_Persona = getIntegerOr(utente, "Flag_Azienda_Persona", 2)

                               If datiBaseUtente.Flag_Azienda_Persona = 1 Then
                                   datiBaseUtente.Descrizione = datiBaseUtente.RagioneSociale
                               Else
                                   datiBaseUtente.Descrizione = datiBaseUtente.Cognome & " " & datiBaseUtente.Nome
                               End If
                               Return datiBaseUtente
                           Catch ex As Exception
                               Dim u = utente
                               Throw
                           End Try
                       End Function
                ).ToList

        End If
        Return listaUtenti
    End Function

    ''' <summary>
    ''' Esegue i controlli pre-salvataggio sui dati degli utenti.
    ''' </summary>
    ''' <param name="Utenti">Gli utenti su cui effettuare i controlli</param>
    ''' <param name="Operazione">istanza di <tt>enum_TipoOperazioneDB</tt> indicante l'operazione da eseguire</param>
    ''' <returns>Una stringa contenente eventuali warning per gli utenti salvati</returns>
    ''' <exception cref="GiasException">Se uno o più utenti tra quelli indicati possiedoni dati non conformi ai requisiti</exception>
    Public Function ControllaUtenti(
        ByRef Utenti As List(Of AgronicaCoreModelsSTD.profilazione.Utente),
        Operazione As enum_TipoOperazioneDB, params As ObjParams
    ) As String
        Dim ErrMsg = String.Empty
        Dim WrnMsg = String.Empty
        For Each Utente In Utenti
            Dim UserErrMsg As IEnumerable(Of ErroreGias) = ControllaDati_Utente(Utente, Operazione, params.ObjParametri_Utenti)
            Dim errore = UserErrMsg.FirstOrDefault(Function(err) err.severity = ErroreGias_Severity.Bloccante)
            Dim warning = UserErrMsg.FirstOrDefault(Function(err) err.severity = ErroreGias_Severity.Warning)

            If gestioneSpidAbilitata(params) AndAlso
                    Not String.IsNullOrWhiteSpace(Utente.CF_SPID) AndAlso
                    Not VerificaEspressioneRegolare(Utente.CF_SPID, "", enum_EspressioniRegolari.RegExp_CodiceFiscale) Then

                errore.messaggio += ResProfilazione.ErrCfSpidFormato
            End If

            If errore.messaggio <> "" Then
                ErrMsg += Utente.UserName & errore.messaggio & vbCrLf
            End If
            If warning.messaggio <> "" Then
                WrnMsg += Utente.UserName & warning.messaggio & vbCrLf
            End If
        Next
        If Operazione = enum_TipoOperazioneDB.Scrittura Then
            ControllaNuoviUtentiDatiNonRipetuti(Utenti, ErrMsg)
        End If
        If ErrMsg <> "" Then
            Throw New GiasException(ErrMsg)
        End If
        Return WrnMsg
    End Function

    Private Function ControllaDati_Utente(ByVal User As AgronicaCoreModelsSTD.profilazione.Utente,
                                          ByVal Operazione As enum_TipoOperazioneDB,
                                          objParametri_Utenti As AgronicaCoreParametri) As IEnumerable(Of ErroreGias)
        Dim ASG_Username = objParametri_Utenti.UsernameOperazione
        Dim errore As New ErroreGias With {
            .messaggio = String.Empty,
            .severity = ErroreGias_Severity.Bloccante
        }
        Dim warning As New ErroreGias With {
            .messaggio = String.Empty,
            .severity = ErroreGias_Severity.Warning
        }

        '----- Verifica Note Utente (Ex Qualifica) non nullo avviene già su Angular
        '----- Verifica permessi: permessi asseganbili solo ai profili. Il controllo per l'assegnazione di un profilo avviene su angular.
        '----- Verifica validià permessi
        ControllaDatePermessi(ASG_Username, User, errore.messaggio, objParametri_Utenti)
        '----- Verifica validià username
        ControllaUsername(User.UserName, Operazione, errore.messaggio, objParametri_Utenti)
        ControllaEmail(User, Operazione, errore.messaggio, warning.messaggio, objParametri_Utenti)

        If Operazione = enum_TipoOperazioneDB.Modifica Then
            Return New List(Of ErroreGias) From {errore, warning}
        End If


        '----- Verifica correttezza tipo utente
        If User.Azienda_Persona = "I" OrElse User.Azienda_Persona = "Impresa" Then
            User.Azienda_Persona = "I"
            User.PIVA = User.CodFisc
            User.CodFisc = ""
            User.Rag_Soc = User.Cognome
            User.Cognome = ""
            User.Nome = ""
            ControllaImpresa(User.PIVA, errore.messaggio, objParametri_Utenti)
        ElseIf User.Azienda_Persona = "P" OrElse User.Azienda_Persona = "Persona" Then
            User.Azienda_Persona = "P"
            ControllaPersona(User.Nome, User.CodFisc, errore.messaggio, objParametri_Utenti)
        End If

        Return New List(Of ErroreGias) From {errore, warning}
    End Function

    Private Sub ControllaNuoviUtentiDatiNonRipetuti(utenti As IEnumerable(Of AgronicaCoreModelsSTD.profilazione.Utente), ByRef Err As String)
        Dim hasUsersSameUsername = utenti.GroupBy(Function(user) user.UserName).
            Where(Function(sameUsername) sameUsername.Count > 1).Any
        Dim hasUsersSameCF = utenti.GroupBy(Function(user) user.CodFisc).
            Where(Function(sameCF) sameCF.Count > 1).Any
        Dim hasUsersSameEmail = utenti.GroupBy(Function(user) user.Email).
            Where(Function(sameCF) sameCF.Count > 1).Any

        If hasUsersSameUsername Then
            Err += ResProfilazione.ErrInserimentoNuoviUtentiStessoUsername
        End If
        If hasUsersSameCF Then
            Err += ResProfilazione.ErrInserimentoNuoviUtentiStessoCF
        End If
        If hasUsersSameEmail Then
            Err += ResProfilazione.ErrInserimentoNuoviUtentiStessaEmail
        End If
    End Sub

    Private Sub ControllaDatePermessi(ByVal Username As String, ByVal user As AgronicaCoreModelsSTD.profilazione.Utente, ByRef Err As String,
                                           objParametri_Utenti As AgronicaCoreParametri)

        If IsNothing(user.Validita_Inizio) Then
            Err += ResProfilazione.ErrDataInizioNull
        End If
        If IsNothing(user.Validita_Fine) Then
            Err += ResProfilazione.ErrDataFineNull
        End If

        If Not IsNothing(user.Validita_Inizio) AndAlso Not IsNothing(user.Validita_Fine) Then
            Dim MinDate As Date = AGRODATAINIZIO
            Dim MaxDate As Date = AGRODATAFINE

            'Recupero estremi
            Dim ObjPermessiDAL As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim DtPermessi As DataTable
            DtPermessi = ObjPermessiDAL.Leggi(Username, enum_Id_Servizio.GiasOnline, 0, 9999, 0, "", "", objParametri_Utenti)

            If Not DtPermessi Is Nothing AndAlso DtPermessi.Rows.Count > 0 Then
                MinDate = CDate(DtPermessi.Rows(0).Item("Validita_Inizio"))
                MaxDate = CDate(DtPermessi.Rows(0).Item("Validita_Fine"))
            End If

            'Verifico estremi
            If CDate(user.Validita_Inizio) < MinDate Then
                Err += ResProfilazione.ErrDataInizioRange.Replace("{0}", MinDate)
            End If
            If CDate(user.Validita_Fine) > MaxDate Then
                Err += ResProfilazione.ErrDataFineRange.Replace("{0}", MaxDate)
            End If
        End If
    End Sub

    Public Function LeggiUtentiDaUsername(ByVal userName As String,
                                          ByRef objParametri_Utenti As AgronicaCoreParametri
                                          ) As DataTable

        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_Read

        Dim DtUtenti As DataTable

        DtUtenti = objUtentiDAL.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta,
                                      String.Format(" Utenti.UserName = '{0}' ", Agro_SQL_SaveText(userName)),
                                      "", objParametri_Utenti)

        Return DtUtenti
    End Function

    Public Function VerificaEsistenzaUtente(ByVal email As String, ByVal CF As String, ByVal PIVA As String,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri
                                            ) As String

        Dim objUtentiDettagliDAL As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R

        Dim DtDettagliUtenti As DataTable

        DtDettagliUtenti = objUtentiDettagliDAL.Leggi("", 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                      String.Format(" Email = '{0}' ", Agro_SQL_SaveText(email)),
                                                      "", objParametri_Utenti)

        If DtDettagliUtenti IsNot Nothing AndAlso DtDettagliUtenti.Rows.Count > 0 Then
            Return "EMAIL"
        End If

        If Not CF.Equals("") Then
            DtDettagliUtenti = objUtentiDettagliDAL.Leggi("", 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                          String.Format(" CodFisc = '{0}' ", Agro_SQL_SaveText(CF)),
                                                          "", objParametri_Utenti)

            If DtDettagliUtenti IsNot Nothing AndAlso DtDettagliUtenti.Rows.Count > 0 Then
                Return "CF"
            End If
        End If

        If Not PIVA.Equals("") Then
            DtDettagliUtenti = objUtentiDettagliDAL.Leggi("", 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                          String.Format(" PIVA = '{0}' ", Agro_SQL_SaveText(PIVA)),
                                                          "", objParametri_Utenti)

            If DtDettagliUtenti IsNot Nothing AndAlso DtDettagliUtenti.Rows.Count > 0 Then
                Return "PIVA"
            End If
        End If

        Return "OK"
    End Function

    Public Function usernameUsato(username As String, objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_Read
        username = username.ToLower()
        Dim DtUtenti As DataTable = objUtentiDAL.Leggi(
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            "Utenti.UserName = '" & Agro_SQL_SaveText(username) & "' ",
            "", objParametri_Utenti
        )
        Return DtUtenti IsNot Nothing AndAlso DtUtenti.Rows.Count > 0
    End Function

    Public Function LeggiUtenteDaEmail(ByVal email As String,
                                       ByRef objParametri_Utenti As AgronicaCoreParametri
                                      ) As DataTable

        Dim objUtentiDettagliDAL As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_Read

        Dim DtUtenti As DataTable

        DtUtenti = objUtentiDettagliDAL.Leggi("", 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                      String.Format(" Email = '{0}' ", Agro_SQL_SaveText(email)),
                                                      "", objParametri_Utenti)

        If DtUtenti Is Nothing OrElse DtUtenti.Rows.Count <> 1 Then
            Return DtUtenti
        End If

        Dim userName = DtUtenti.Rows(0)("UserName").ToString

        DtUtenti = objUtentiDAL.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta,
                                      String.Format(" Utenti.UserName = '{0}' ", Agro_SQL_SaveText(userName)),
                                      "", objParametri_Utenti)

        Return DtUtenti
    End Function

    Public Function LeggiUtentiDettagliDaUserName(ByVal userName As String,
                                                  ByRef objParametri_Utenti As AgronicaCoreParametri) As DataTable

        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim DT As DataTable

        DT = xRead.Leggi(userName, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

        Return DT
    End Function

    Public Function LeggiUtentiDettagliDaCodFisc(ByVal CF As String,
                                                  ByRef objParametri_Utenti As AgronicaCoreParametri) As DataTable

        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim DT As DataTable

        DT = xRead.Leggi("", 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                         String.Format(" CodFisc = '{0}' ", Agro_SQL_SaveText(CF)),
                         "", objParametri_Utenti)

        Return DT
    End Function

    ''' <summary>
    ''' Legge nome, cognome e codice fiscale degli utenti specificati.
    ''' </summary>
    ''' <param name="utenti">Usernames degli utenti interessai. Se lista vuota, vengono letti tutti gli utenti.</param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <returns>Lista di UtenteDTO</returns>
    Public Function LeggiDettagliMinimi(utenti As List(Of String), objParametri_Utenti As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.profilazione.UtenteDTO)
        Dim objDett As New Utenti_Dettagli_R

        Dim filtro As New Text.StringBuilder With {.Length = 0}
        If utenti.Any() Then
            filtro.AppendLine(" AND UserName in ( ")
            filtro.Append(utenti.Aggregate(Function(u1, u2) u1 & ", " & u2))
            filtro.Append(" ) ")
        End If

        Dim DTdett = objDett.Leggi("", enum_Id_Servizio.GiasOnline,
                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                filtro.ToString(), "",
                                objParametri_Utenti)

        Return DTdett.Select.AsParallel.
            Select(Function(row) New AgronicaCoreModelsSTD.profilazione.UtenteDTO With {
                .UserName = row.Item("UserName"),
                .Nome = row.Item("Nome"),
                .Cognome = row.Item("Cognome"),
                .codice_fiscale = row.Item("CodFisc")
            }).ToList()
    End Function

    Private Sub ControllaUsername(
        ByRef Username As String, ByVal Operazione As enum_TipoOperazioneDB,
        ByRef Err As String, objParametri_Utenti As AgronicaCoreParametri
    )
        Username = Username.ToLower

        If Username = "" Then
            Err += ResProfilazione.ErrUsernameNull
        ElseIf Username.Contains(" ") Then
            Err += ResProfilazione.ErrUsernameSpazio
        ElseIf Not VerificaEspressioneRegolare(Username, "", enum_EspressioniRegolari.RegExp_Username) Then
            Err += ResProfilazione.ErrUsernameFormato
        End If

        Dim usato = usernameUsato(Username, objParametri_Utenti)
        If Operazione = enum_TipoOperazioneDB.Scrittura AndAlso usato Then
            Err += ResProfilazione.ErrUsernameEsiste
        ElseIf Operazione = enum_TipoOperazioneDB.Modifica AndAlso Not usato Then
            Err += ResProfilazione.ErrUsernameNonEsiste
        End If
    End Sub

    ''' <summary>
    ''' Esegue i controlli sul formato della password assicurandosi che
    ''' rispetti i vincoli stabiliti.
    ''' </summary>
    ''' <param name="user">Utente interessato dal cambio password</param>
    ''' <param name="Operazione">Tipo operazione sull'utente, in particolare
    ''' ci interesasa se si tratta si una modifica o di una nuova scrittura.</param>
    ''' <param name="Err">Stringa attraverso cui restituire eventuali errori</param>
    ''' <remarks>
    ''' Se la password non è valorzzata e sono in modifica dell'utente, il campo 
    ''' viene valorizzato con una stringa vuota. Necessario poi gestire la cosa
    ''' in fase di modifica vera e propria del record.
    ''' </remarks>
    Private Sub ControllaPassword(ByRef user As IUtentePassword, ByVal Operazione As enum_TipoOperazioneDB,
                                  ByRef Err As String, objParametri_Utenti As AgronicaCoreParametri)
        If String.IsNullOrWhiteSpace(user.Password) AndAlso Operazione = enum_TipoOperazioneDB.Modifica Then
            ' Sto modificando i dati dell'utente ma non la sua password
            ' => salto i controlli ma valorizzo comunque il campo per evitare problemi
            user.Password = String.Empty
            Return
        End If

        Dim dettagli As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim minPwdLength = 8
        If String.IsNullOrWhiteSpace(user.Password) Then
            Err += ResProfilazione.ErrPasswordNull
        ElseIf user.Password.Count < minPwdLength Then
            Err += ResProfilazione.ErrPasswordMinLength
        ElseIf Not VerificaEspressioneRegolare(user.Password, "", enum_EspressioniRegolari.RegExp_Password) Then
            Err += ResProfilazione.ErrPasswordFormato
        End If
        If Operazione = enum_TipoOperazioneDB.Modifica Then
            Dim dt = dettagli.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta, String.Empty, String.Empty, objParametri_Utenti, user.UserName)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 AndAlso dt.Rows(0)("Password") = user.Password Then
                Err += ResProfilazione.ErrPasswordSamePrev
            End If
        End If
    End Sub

    Private Sub ControllaEmail(ByVal Utente As AgronicaCoreModelsSTD.profilazione.Utente, ByVal Operazione As enum_TipoOperazioneDB,
                               ByRef Err As String, ByRef warning As String, objParametri_Utenti As AgronicaCoreParametri)
        Dim notice = String.Empty
        If String.IsNullOrWhiteSpace(Utente.Email) Then
            notice += ResProfilazione.ErrEmailNull
        Else
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
            Dim users = objUtenti.Utenti_From_Email(Utente.Email, objParametri_Utenti)
            If (users.Rows.Count > 1) OrElse (users.Rows.Count = 1 AndAlso CStr(users.Rows(0)("UserName")).ToUpper() <> Utente.UserName.ToUpper()) Then
                notice += ResProfilazione.ErrEmailEsiste
            End If
        End If

        If Operazione = enum_TipoOperazioneDB.Modifica OrElse Utente.Flag_Accesso_SPID Then
            warning += notice
        Else
            Err += notice
        End If
    End Sub

    Private Sub ControllaPersona(Nome As String, CF As String, ByRef Err As String, objParametri_Utenti As AgronicaCoreParametri)
        '----- Verifica Cognome non nullo avviene già su Angular
        If Nome = "" Then
            Err += ResProfilazione.ErrNomeCognomeNull
        End If
        '----- Verifica formato CF
        Dim permission As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim canWrite = permission.Controlla_Permessi_Utente(
            objParametri_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline, enum_Security_Attivita.UtentiGenerazioneCF,
            enum_Security_Operazione.Modifica, Now, String.Empty, objParametri_Utenti
        )
        Dim isCFAutogen = canWrite AndAlso VerificaEspressioneRegolare(CF, "^CF([0-9]{14})$", enum_EspressioniRegolari.RegExp_Nessuna)

        If Not VerificaEspressioneRegolare(CF, "", enum_EspressioniRegolari.RegExp_CodiceFiscale) AndAlso Not isCFAutogen Then
            Err += ResProfilazione.ErrCfFormato
            Exit Sub
        End If
        '----- Verifica CF già in archivio
        Dim objUtentiDettagli As New Utenti_Dettagli_R
        Dim DtUtentiDettagli As DataTable
        DtUtentiDettagli = objUtentiDettagli.Leggi(
            "", 0, enumSelezioneVariabile.Selezione_JoinDescrizioni,
            " Utenti_Dettagli.CodFisc = '" & Agro_SQL_SaveText(CF) & "' ",
            "", objParametri_Utenti
        )
        If Not DtUtentiDettagli Is Nothing AndAlso DtUtentiDettagli.Rows.Count > 0 Then
            Err += ResProfilazione.ErrCfEsiste
        End If
    End Sub

    Private Sub ControllaImpresa(PIVA As String, ByRef Err As String, objParametri_Utenti As AgronicaCoreParametri)
        '----- Verifica Ragione Sociale non nulla avviene già su Angular
        '----- Verifica formato PIVA
        If Not VerificaEspressioneRegolare(PIVA, "", enum_EspressioniRegolari.RegExp_PartitaIVA) Then
            Err += ResProfilazione.ErrPivaFormato
            Exit Sub
        End If
        '----- Verifica PIVA già in archivio
        Dim objUtentiDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim DtUtentiDettagli As DataTable
        DtUtentiDettagli = objUtentiDettagli.Leggi(
            "", 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
            " ( Utenti_Dettagli.PIVA = '" & Agro_SQL_SaveText(PIVA) & "' OR Utenti_Dettagli.CodFisc = '" & Agro_SQL_SaveText(PIVA) & "' ) ",
            "", objParametri_Utenti
        )
        If Not DtUtentiDettagli Is Nothing AndAlso DtUtentiDettagli.Rows.Count > 0 Then
            Err += ResProfilazione.ErrPivaEsiste
        End If
    End Sub

    Public Function IsAlive(ByRef objP_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim utenti_R As New AgronicaCoreUtentiDAL.Utenti_Read
            r.RispostaOK = utenti_R.IsAlive(objP_Utenti)
            r.Errore = If(r.RispostaOK, "", Gias.DB_Utenti_Unreachable)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gias.DB_Utenti_Unreachable
        End Try

        Return r

    End Function

    Public Sub InizializzaLayersGis(utenti As List(Of AgronicaCoreModelsSTD.profilazione.Utente),
                                    objParametri_Server As AgronicaCoreParametri,
                                    objParametri_Utenti As AgronicaCoreParametri)
        Dim prepareFilter = Function(acc, u)
                                If u.index Mod 100 = 0 Then
                                    acc.name &= ", --" & u.index & vbNewLine & u.name
                                ElseIf u.index Mod 10 = 0 Then
                                    acc.name &= ", " & vbNewLine & u.name
                                Else
                                    acc.name &= ", " & u.name
                                End If
                                Return acc
                            End Function
        Dim usernamesBatch = utenti.AsParallel.
                    Select(Function(utente) New UtentePermessi With {
                        .UserName = utente.UserName,
                        .Tipologia = New TipologiaUtente() With {.codice = utente.Tipologia_Cod}
                    }).Where(Function(dto) HaPermessoCartografiaAziendale(dto.Tipologia, objParametri_Utenti)).
                    Select(Function(u, i) New With {.name = "'" & u.UserName & "'", .index = i}).
                    GroupBy(Function(u) u.index \ 10_000).
                    Select(Function(batch) batch.Aggregate(prepareFilter).name). ' lista di usernames 
                    ToList
        usernamesBatch.ForEach(Sub(batch) InitGisLayersFromProfile(batch, objParametri_Utenti, objParametri_Server))
    End Sub

    Public Sub ScriviUtenti(
        Utenti As List(Of AgronicaCoreModelsSTD.profilazione.Utente),
        Operazione As enum_TipoOperazioneDB, params As ObjParams,
        Optional ignoraPermessi As Boolean = False,
        Optional impostazioniDaProfilo As Boolean = False
    )
        Dim ErrMsg As String = ""
        Dim ASG_Username = params.ObjParametri_Utenti.UsernameOperazione
        Try
            '----- APRO CONNESSIONE AL DATABASE
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, params.ObjParametri_Utenti)

            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim objVisibilita As New AgronicaCoreUtentiBIZ.Utenti_Visibilita
            Dim objGruppiUt As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_W
            Dim dtConfigSiti = objConfigSiti.Leggi(0, "AbilitaHashPassword",
                                                   "", "", params.ObjParametri_Server)
            Dim gestioneHashAbilita As Boolean = False
            If dtConfigSiti.Rows.Count > 0 Then
                gestioneHashAbilita = dtConfigSiti.Rows(0)("Valore")
            End If

            For Each Utente In Utenti
                Utente.UserName = Utente.UserName.ToLower
                Dim dto = New UtentePermessi With {
                    .UserName = Utente.UserName,
                    .Tipologia = New TipologiaUtente() With {.codice = Utente.Tipologia_Cod},
                    .ValiditaInizioPermessi = Utente.Validita_Inizio, 'If(Utente.Data_Inizio <> "...", CDate(Utente.Data_Inizio), AGRODATAINIZIO),
                    .ValiditaFinePermessi = Utente.Validita_Fine 'If(Utente.Data_Fine <> "...", CDate(Utente.Data_Fine), AGRODATAFINE)
                }

                If Operazione = enum_TipoOperazioneDB.Modifica Then
                    '----- Creo/Aggiorno dati base
                    Modifica_Utente(Utente, enum_Id_Servizio.GiasOnline, params.ObjParametri_Utenti, gestioneHashAbilita)
                Else
                    Scrivi_NuovoUtente(Utente, gestioneHashAbilita,
                                           params.ObjParametri_Utenti, params.ObjParametri_Server)
                    ''22/11/23: creo il profilo visibilità con visibilità nulla
                    objVisibilita.AssegnaVisibilitaNulla(
                                New UtenteDTO With {.UserName = Utente.UserName},
                                params.ObjParametri_Server, params.ObjParametri_Utenti
                            )
                End If

                '----- Aggiorno permessi collegati al profilo
                If Not ignoraPermessi Then
                    'Se sono in scrittura, utilizzo le date indicate nel form di creazione => non calcolo la validità
                    AggiornaPermessi(dto, params.ObjParametri_Utenti,
                                     calcolaValidita:=Operazione <> enum_TipoOperazioneDB.Scrittura)
                End If

                If Operazione = enum_TipoOperazioneDB.Scrittura AndAlso impostazioniDaProfilo Then
                    AssociaImpostazioniProfilo(dto, params.ObjParametri_Utenti)
                End If

                '----- Aggiorno gruppi
                If Utente.GruppiCod <> "" Then
                    objGruppiUt.Cancella(0, Utente.UserName, "", params.ObjParametri_Utenti)

                    Dim gruppi = Utente.GruppiCod.Split("|").
                        Select(Function(str) Convert.ToInt32(str)).
                        Where(Function(x) x <> 0).
                        GetEnumerator()
                    While gruppi.MoveNext
                        objGruppiUt.Scrivi(
                            gruppi.Current, Utente.UserName,
                            AGRODATAINIZIO, AGRODATAFINE,
                            params.ObjParametri_Utenti
                        )
                    End While
                End If

                If gestioneSpidAbilitata(params) Then
                    ScriviDatiSpid(Utente, dto.ValiditaInizioPermessi, dto.ValiditaFinePermessi, params.ObjParametri_Utenti)
                End If


            Next

            '----- CHIUDO CONNESSIONE AL DATABASE
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, params.ObjParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(params.ObjParametri_Utenti)

            Dim gestoreCache As New AgronicaCoreVarieBIZ.GestoreCache
            Dim client As New HttpClient()

            gestoreCache.PulisciCachePermessi(client, params.ObjParametri_Server)

            If impostazioniDaProfilo Then
                gestoreCache.PulisciCacheImpostazioni(client, params.ObjParametri_Server)
            End If

        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, params.ObjParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(params.ObjParametri_Utenti)
            Throw ex
        End Try
    End Sub

    Public Sub ScriviDatiSpid(utente As AgronicaCoreModelsSTD.profilazione.Utente,
                              validitaInizio As DateTime, validitaFine As DateTime,
                              objParametri_Utenti As AgronicaCoreParametri)
        Dim scriviUtenti As New AgronicaCoreUtentiDAL.Utenti_Write
        Dim objCF_W As New AgronicaCoreUtentiDAL.UtentixCodFisc_W
        Dim objCF_R As New AgronicaCoreUtentiDAL.UtentixCodFisc_R
        Dim oldData = New UtenteAccessoSPID() With {
            .UserName = utente.UserName,
            .CfLogin = If(String.IsNullOrEmpty(utente.CF_SPID_Corrente), String.Empty, utente.CF_SPID_Corrente)
        }
        Dim newData = New UtenteAccessoSPID() With {
            .UserName = utente.UserName,
            .CfLogin = If(String.IsNullOrEmpty(utente.CF_SPID), String.Empty, utente.CF_SPID).ToUpper
        }
        scriviUtenti.ModificaFlagSPID(utente.UserName, utente.Flag_Accesso_SPID, objParametri_Utenti)
        Dim cfUtente = objCF_R.Leggi(utente.UserName, utente.CF_SPID_Corrente,
            String.Empty, String.Empty, objParametri_Utenti)

        If String.IsNullOrEmpty(newData.CfLogin) Then
            Return
        End If

        If cfUtente.Rows.Count > 0 Then
            objCF_W.Modifica(oldData, newData, objParametri_Utenti)
        Else
            objCF_W.Scrivi(newData.UserName, newData.CfLogin,
                validitaInizio, validitaFine, objParametri_Utenti)
        End If
    End Sub

    Public Sub AggiornaFinestraTemporale(utenti As IEnumerable(Of IUtenteFinestraTemp), objServer As AgronicaCoreParametri, objUtenti As AgronicaCoreParametri)
        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objServer)
            utenti.ToList.ForEach(Sub(u) AggiornaFinestraTemporale(u, objServer, objUtenti))
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objServer)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objServer)
        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objServer)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objServer)
        End Try
    End Sub

    Private Sub AggiornaFinestraTemporale(user As IUtenteFinestraTemp, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)
        Dim objFinestraTemp_R As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim scriviUtenti As New AgronicaCoreUtentiDAL.Utenti_Write
        Dim userTemp = objFinestraTemp_R.Leggi_Da_Gias_Server(
            user.UserName, enumSelezioneVariabile.Selezione_TabellaCompleta,
            xFiltroAggiuntivo:=String.Empty, xOrderBy:=String.Empty, objParametri_Server
        ).Select.Select(Function(row) New UtenteFinestraTemp(
            row("USER"),
            New IntervalloTemporale(row("FinestraTemp_Inizio"), row("FinestraTemp_Fine"))
        )).FirstOrDefault
        If userTemp IsNot Nothing Then
            scriviUtenti.Modifica_FinestraTemporale(user.UserName, user.FinestraTemporale.inizio, user.FinestraTemporale.fine, objParametri_Server)
        Else
            Dim details As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
            Dim codFisc = details.CodFisc_From_Username(user.UserName, objParametri_Utenti)
            scriviUtenti.Utenti_GiasServer_Scrivi(
                NumeroRecordInteressati:=1, user.UserName,
                user.FinestraTemporale.inizio, user.FinestraTemporale.fine,
                Codice_Fiscale:=codFisc, Nome_Resp:=String.Empty, Ente_Resp:=0,
                Inizio_Attivita:=Date.Today, Attivita:=0, Calcolatore:=0, Password:="", Tipo:=0,
                AGRODATAINIZIO, AGRODATAFINE,
                objParametri_Server
            )
        End If
    End Sub

    Public Function Modifica_Email_Utente_Retail(ByVal User As Object,
                                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As Boolean

        Dim objDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W

        Modifica_Utente(User, TipiEnumerativi.enum_Id_Servizio.GiasOnline, objParametri_Utenti)

        Return True
    End Function

    Public Function Scrivi_Utente_Retail(ByVal User As JObject,
                                         ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As RispostaStandard

        Dim resp As New RispostaStandard

        Dim objUtentiWrite As New AgronicaCoreUtentiDAL.Utenti_Write

        Try

            resp.RispostaOK = objUtentiWrite.Scrivi(User("UserName").ToString,
                                                    User("Password").ToString,
                                                    0, True, True,
                                                    objParametri_Utenti)

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nella creazione dell'utente.")
            End If

            Dim objDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W

            resp.RispostaOK = objDettagli.Scrivi(User("UserName").ToString, User("Cognome").ToString, User("Nome").ToString, "", "", "", "", "",
                                                 User("Tel").ToString, "", User("Email").ToString, User("PIVA").ToString,
                                                 User("CodFisc").ToString, User("Rag_Soc").ToString, 2, 0,
                                                 User("UserNameCommerciale").ToString, objParametri_Utenti)

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nella scrittura dei dettagli dell'utente.")
            End If

            Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Write
            Dim listaPivaLette = New List(Of String)
            listaPivaLette.Add(User("PIVA").ToString)

            Dim profilatore As New ProfilatoreUtenze

            Dim Descrizione_1 As String = ""
            Dim Descrizione_2 As String = ""
            profilatore.impostaVisibilitaUtente_descr1descr2(listaPivaLette, Descrizione_1, Descrizione_2)

            'Inserisco profilo x giasonline
            resp.RispostaOK = objProfilo.Scrivi(User("UserName").ToString, TipiEnumerativi.enum_Id_Servizio.GiasOnline,
                                                Descrizione_1, Descrizione_2, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                objParametri_Utenti)

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nella scrittura dei profili dell'utente.")
            End If

            'Inserisco profilo x giaslan
            resp.RispostaOK = objProfilo.Scrivi(User("UserName").ToString, TipiEnumerativi.enum_Id_Servizio.GiasLAN,
                                                "", "", 0, 0, AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nella scrittura dei profili dell'utente.")
            End If

            resp.RispostaOK = objDettagli.Modifica_PivaSuperUserUtente(User("UserName").ToString, objParametri_Utenti)

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nel settaggio della PivaSuperUser per l'utente creato.")
            End If

            resp.RispostaStringa = "Operazione terminata con successo."

        Catch ex As Exception
            resp.RispostaOK = False
            resp.Errore = ex.Message
        End Try

        Return resp
    End Function

    Public Function ImpostaPermessi_Retail(ByVal User As JObject,
                                            ByVal idServizio As Int32,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Boolean
        Dim resp As Boolean

        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W
        Dim objPermesso_W As New AgronicaCoreUtentiDAL.Utenti_Permessi_W
        Dim objTipologiexPermessi_W As New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_W

        Dim DataInizio As Date = AGRODATAINIZIO
        Dim DataFine As Date = AGRODATAFINE

        If User("Data_Inizio") IsNot Nothing AndAlso Not User("Data_Inizio").ToString.Equals("") Then
            DataInizio = CDate(User("Data_Inizio").ToString)
        End If

        If User("Data_Fine") IsNot Nothing AndAlso Not User("Data_Fine").ToString.Equals("") Then
            DataFine = CDate(User("Data_Fine").ToString)
        End If

        '----- Cancellazione di tutti i PERMESSI
        resp = objPermesso_W.Cancella(User("UserName").ToString, idServizio, 0, 0, 0, "", objParametri_Utenti)

        If Not resp Then
            Throw New Exception("Errore nella cancellazione dei permessi dell'utente")
        End If

        '----- Se non è stata assegnata una tipologia esce dalla funzione
        If User("Tipologia_Cod") Is Nothing OrElse User("Tipologia_Cod").ToString.Equals("") Then
            Throw New Exception("Specificare il codice tipologia per l'utente retail")
        End If

        resp = Convert.ToBoolean(objUtentiDAL.Modifica_TipologiaUtente(User("UserName").ToString,
                                                                       User("Tipologia_Cod").ToString,
                                                                       objParametri_Utenti))

        If Not resp Then
            Throw New Exception("Errore nell'assegnazione della tipologia all'utente")
        End If

        '----- Genera nuovi permessi legati alla tipologia in tabella Utenti_Permessi
        resp = objTipologiexPermessi_W.GeneraPermessiUtenteDaTipologia(User("Tipologia_Cod").ToString,
                                                                       User("UserName").ToString,
                                                                       DataInizio, DataFine,
                                                                       objParametri_Utenti)

        If Not resp Then
            Throw New Exception("Errore nella generazione dei permessi per l'utente")
        End If

        resp = objTipologiexPermessi_W.GeneraImpostazioniUtenteDaTipologia(CInt(User("Tipologia_Cod")),
                                                                           User("UserName").ToString,
                                                                           DataInizio, DataFine,
                                                                           String.Format(" i.Impostazione_Cod <> {0} ", CInt(enum_Impostazioni_Utenti.UTENTE_Attiva_Configurazione_Pratica).ToString),
                                                                           objParametri_Utenti)

        If Not resp Then
            Throw New Exception("Errore nella generazione dele impostazioni per l'utente")
        End If

        resp = objTipologiexPermessi_W.GeneraImpostazioniUtenteFiltroMonoDaTipologia(CInt(User("Tipologia_Cod")),
                                                                                     User("UserName").ToString,
                                                                                     DataInizio, DataFine,
                                                                                     objParametri_Utenti)

        If Not resp Then
            Throw New Exception("Errore nella generazione dele impostazioni per l'utente")
        End If

        Dim gestoreCache As New AgronicaCoreVarieBIZ.GestoreCache
        Dim client As New HttpClient()

        gestoreCache.PulisciCachePermessi(client, objParametri_Server)
        gestoreCache.PulisciCacheImpostazioni(client, objParametri_Server)

        Return resp
    End Function

    Private Sub Scrivi_NuovoUtente(User As AgronicaCoreModelsSTD.profilazione.Utente, gestioneHashAbilita As Boolean,
                                   objParametri_Utenti As AgronicaCoreParametri,
                                   objParametri_Server As AgronicaCoreParametri)
        Dim objUtentiWrite As New AgronicaCoreUtentiDAL.Utenti_Write
        Dim Flag_Persona = 2
        Dim Flag_Azienda = 1
        '----- Record Utente
        Dim creazioneOk = objUtentiWrite.Scrivi(User.UserName, User.Password, 0,
                                          gestioneHashAbilita, False,
                                          objParametri_Utenti, User.Tipologia_Cod)
        '----- Record Dettagli
        If creazioneOk = True Then
            Dim objDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W

            Select Case User.Azienda_Persona
                Case "I"
                    User.CodFisc = If(String.IsNullOrEmpty(User.CodFisc), User.PIVA, User.CodFisc)
                    objDettagli.Scrivi(User.UserName, String.Empty, String.Empty,
                                       User.Tel, User.Email, User.PIVA, User.CodFisc, User.Rag_Soc,
                                       Flag_Azienda, User.UserNameCommerciale, objParametri_Utenti)
                Case "P"
                    objDettagli.Scrivi(User.UserName, User.Cognome, User.Nome,
                                      User.Tel, User.Email, User.PIVA, User.CodFisc.ToUpper, String.Empty,
                                      Flag_Persona, User.UserNameCommerciale, objParametri_Utenti)
                Case Else
                    Throw New ArgumentException("Il campo Azienda_Persona dell'utente deve esserre valorizzato
                        con I o P. Il valore letto per l'utente " & User.UserName & " è: " & User.Azienda_Persona)
            End Select

            objDettagli.Modifica_PivaSuperUserUtente(User.UserName, objParametri_Utenti)

            '22/11/23: creo il profilo visibilità con visibilità nulla -- spostato in entry point chiamata
        End If
    End Sub

    Private Sub Modifica_Utente(
        User As AgronicaCoreModelsSTD.profilazione.Utente,
        ASG_IdServizio As Integer,
        objParametri_Utenti As AgronicaCoreParametri,
        Optional gestioneHashAbilitata As Boolean = False
    )
        Dim objUtentiWrite As New AgronicaCoreUtentiDAL.Utenti_Write
        Dim objUtentiRead As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim objDettagli As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W
        Dim objPermesso_W As New AgronicaCoreUtentiDAL.Utenti_Permessi_W
        Dim Flag_Persona = 2
        Dim Flag_Azienda = 1

        '----- Record Utente
        If IsNothing(User.Lingua_Cod) OrElse User.Lingua_Cod = "0" Then
            User.Lingua_Cod = enum_AgroLingue.Italiano_it
        End If
        objUtentiWrite.ModificaLingua(User.UserName, User.Lingua_Cod, objParametri_Utenti)

        If Not String.IsNullOrEmpty(User.Password) Then
            Dim old = objUtentiRead.Password_From_UserName(User.UserName, objParametri_Utenti)
            If old <> User.Password Then
                objUtentiWrite.Modifica(User.UserName, User.Password, gestioneHashAbilitata, False, objParametri_Utenti)
            End If
        End If

        '----- Record Dettagli
        Select Case User.Flag_Azienda_Persona
            Case Flag_Azienda
                Dim pivaOrCf = If(String.IsNullOrWhiteSpace(User.PIVA), User.CodFisc, User.PIVA)
                objDettagli.ModificaDettagliBaseAzienda(
                    User.UserName, pivaOrCf, User.Rag_Soc, User.Tel, User.Email, pivaOrCf,
                    User.UserNameCommerciale, objParametri_Utenti
                )
            Case Flag_Persona
                objDettagli.ModificaDettagliBasePersona(
                    User.UserName, User.Cognome, User.Nome, User.Tel, User.Email,
                    User.CodFisc, User.UserNameCommerciale, objParametri_Utenti
                )
            Case Else
                Throw New Exception("Formato flag azienda/persona non corretto.")
        End Select
    End Sub
#End Region

#Region "Permessi"

    ''' <summary>
    ''' Aggiorna i permessi dell'utente specificato.
    ''' </summary>
    ''' <param name="utente">Utente per cui si vogliono modififcare i permessi</param>
    ''' <param name="objParametri_Utenti">Dati relativi al DB di riferimento</param>
    ''' <param name="calcolaValidita">Se `False` utilizza l'intervallo di validità indicato nell'utente specificato,
    ''' altrimenti lo calcola in base ai permessi già posseduti, prendendo come riferimento la Validita_Inizio minore e la Validita_Fine maggiore</param>
    Private Sub AggiornaPermessi(utente As UtentePermessi, objParametri_Utenti As AgronicaCoreParametri, Optional calcolaValidita As Boolean = True)
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_Dettagli_W
        Dim objPermesso_W As New AgronicaCoreUtentiDAL.Utenti_Permessi_W
        Dim objTipologiexPermessi_W As New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_W

        If calcolaValidita Then
            Dim objPermesso_R As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim tutti = 0, noFiltro = ""
            Dim dt = objPermesso_R.Leggi(utente.UserName, enum_Id_Servizio.GiasOnline,
                                         tutti, tutti, tutti, noFiltro, noFiltro,
                                         objParametri_Utenti)
            If dt.Rows.Count > 0 Then
                utente.ValiditaInizioPermessi = dt.AsEnumerable.Min(Function(u) u.Item("Validita_Inizio"))
                utente.ValiditaFinePermessi = dt.AsEnumerable.Max(Function(u) u.Item("Validita_Fine"))
            Else
                utente.ValiditaInizioPermessi = AGRODATAINIZIO
                utente.ValiditaFinePermessi = AGRODATAFINE
            End If
        End If

        If IsNothing(utente.Tipologia) OrElse IsNothing(utente.Tipologia.codice) OrElse utente.Tipologia.codice = 0 Then
            objPermesso_W.Modifica_Validita(
                utente.UserName, enum_Id_Servizio.GiasOnline,
                utente.ValiditaInizioPermessi, utente.ValiditaFinePermessi,
                "", objParametri_Utenti
            )
        Else
            '----- Cancellazione di tutti i PERMESSI
            objPermesso_W.Cancella(utente.UserName, enum_Id_Servizio.GiasOnline, 0, 0, 0, "", objParametri_Utenti)

            objUtentiDAL.Modifica_TipologiaUtente(utente.UserName, utente.Tipologia.codice, objParametri_Utenti)

            '----- Genera nuovi permessi legati alla tipologia in tabella Utenti_Permessi
            objTipologiexPermessi_W.GeneraPermessiUtenteDaTipologia(
                utente.Tipologia.codice, utente.UserName,
                utente.ValiditaInizioPermessi, utente.ValiditaFinePermessi,
                objParametri_Utenti
                )
        End If
    End Sub

    ''' <summary>
    ''' Aggiorna i permessi attivi in installazione per l'utente specificato.
    ''' </summary>
    ''' <param name="utente">Utente per cui si vogliono modififcare i permessi</param>
    ''' <param name="objParametri_Utenti">Dati relativi al DB di riferimento</param>
    Public Sub AggiornaCliente_Permessi(ByVal UserName As String,
                                        ByVal Permessi As List(Of Cliente_Permesso),
                                        objParametri_Utenti As AgronicaCoreParametri)
        Dim permessiBiz As New AgronicaCoreUtentiBIZ.Utenti_Permessi_W
        Dim objPermesso_W As New AgronicaCoreUtentiDAL.Utenti_Permessi_W
        Dim objPermesso_R As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Utenti)
            If Permessi.Any Then
                Dim alreadyPresentR As IEnumerable(Of Integer) = {}
                Dim alreadyPresentW As IEnumerable(Of Integer) = {}
                Dim operazione = Permessi.FirstOrDefault.Id_Operazione

                If operazione = enum_TipoPermesso.DISABILITATO AndAlso Permessi.Exists(Function(perm) perm.Permesso_ID = enum_Security_Attivita.Gest_Menu) Then
                    Throw New GiasException(My.Resources.ResProfilazione.ErrCancellazionePermessoAccessoSuperuser)
                End If

                objPermesso_W.Cancella_Cliente_Permessi(
                    UserName, Permessi.Select(Function(p) p.Id_Attivita),
                    enum_TipoPermesso.DISABILITATO, String.Empty, objParametri_Utenti
                )
                If operazione <> enum_TipoPermesso.DISABILITATO Then
                    For Each permesso In Permessi
                        objPermesso_W.Scrivi_Cliente_Permessi(
                            UserName, permesso.Id_Servizio,
                            permesso.Id_Attivita, permesso.Id_Operazione,
                            AGRODATAINIZIO, AGRODATAFINE,
                            objParametri_Utenti
                        )
                    Next
                    Dim suPerms = objPermesso_R.Leggi(
                        objParametri_Utenti.SuperUserUsername, enum_Id_Servizio.GiasOnline,
                        0, 9999, 0, String.Empty, String.Empty, objParametri_Utenti
                    ).Select.Where(Function(row) Permessi.Any(Function(p) p.Permesso_ID = CInt(row("Id_Attivita")))).
                    Select(Function(row) New Cliente_Permesso(CInt(row("Id_Attivita")), CInt(row("Id_Operazione")), CInt(row("Id_Servizio"))))
                    alreadyPresentR = suPerms.Where(Function(p) p.Id_Operazione = enum_TipoPermesso.LETTURA).Select(Function(p) p.Id_Attivita)
                    alreadyPresentW = suPerms.Where(Function(p) p.Id_Operazione = enum_TipoPermesso.LETTURA_SCRITTURA).Select(Function(p) p.Id_Attivita)
                End If

                If operazione = enum_TipoPermesso.DISABILITATO Then
                    permessiBiz.DisabilitaGlobale(Permessi, enum_TipoPermesso.LETTURA, objParametri_Utenti)
                    permessiBiz.DisabilitaGlobale(Permessi, enum_TipoPermesso.LETTURA_SCRITTURA, objParametri_Utenti)
                ElseIf operazione = enum_TipoPermesso.LETTURA Then
                    permessiBiz.DisabilitaGlobale(Permessi, enum_TipoPermesso.LETTURA_SCRITTURA, objParametri_Utenti)
                    Dim toAdd = Permessi.Where(Function(p) Not alreadyPresentR.Contains(p.Id_Attivita)).Select(Function(p) p.Id_Attivita)
                    objPermesso_W.ScriviMultiplo(
                        objParametri_Utenti.SuperUserUsername, toAdd, enum_TipoPermesso.LETTURA,
                        AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti
                    )
                ElseIf operazione = enum_TipoPermesso.LETTURA_SCRITTURA Then
                    Dim toAdd = Permessi.Where(Function(p) Not alreadyPresentR.Contains(p.Id_Attivita)).Select(Function(p) p.Id_Attivita)
                    objPermesso_W.ScriviMultiplo(
                        objParametri_Utenti.SuperUserUsername, toAdd, enum_TipoPermesso.LETTURA,
                        AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti
                    )
                    toAdd = Permessi.Where(Function(p) Not alreadyPresentW.Contains(p.Id_Attivita)).Select(Function(p) p.Id_Attivita)
                    objPermesso_W.ScriviMultiplo(
                        objParametri_Utenti.SuperUserUsername, toAdd, enum_TipoPermesso.LETTURA_SCRITTURA,
                        AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti
                    )
                End If
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Utenti)
        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Utenti)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Utenti)
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Legge i permessi attivi in installazione dell'utente specificato.
    ''' </summary>
    ''' <param name="utente">Utente dei cui si vogliono legge i permessi</param>
    ''' <param name="objParametri_Utenti">Dati relativi al DB di riferimento</param>
    ''' <returns>Lista di Cliente_Permesso. Se il permesso è
    ''' attivo in lettura e scrittura sono presenti due record nella lista
    ''' (Id_Operazione 0 e 2)</returns>
    ''' <remarks>Tira una NotSupportedException se la tabella non è presente sul database.</remarks>
    Public Function LeggiCliente_Permessi(
        ByVal UserName As String, objParametri_Utenti As AgronicaCoreParametri
    ) As List(Of Cliente_Permesso)
        Dim objPermesso_R As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        If Not objPermesso_R.VerificaEsistenzaTabellaClientePermessi(objParametri_Utenti) Then
            Throw New NotSupportedException("Table Cliente_Permessi missing!")
        End If

        Dim listItems = objPermesso_R.LeggiCliente_Permessi(
            UserName, String.Empty, String.Empty, objParametri_Utenti
            ).Select.Select(Function(dr) New Cliente_Permesso() With {
                .Id_Servizio = dr.Item("Id_Servizio"),
                .Id_Attivita = dr.Item("Id_Attivita"),
                .Id_Operazione = dr.Item("Id_Operazione")
            }).ToList
        Return listItems
    End Function


    ''' <summary>
    ''' Verifica l'esistenza della tabella Cliente_Permessi
    ''' </summary>
    ''' <param name="objParametri_Utenti">Dati relativi al DB di riferimento</param>
    ''' <returns>Un boolean che indica l'esistenza o meno della tabella sul DB specificato.</returns>
    Public Function VerificaEsistenzaCliente_Permessi(objParametri_Utenti As AgronicaCoreParametri
    ) As Boolean
        Dim objPermesso_R As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        Return objPermesso_R.VerificaEsistenzaTabellaClientePermessi(objParametri_Utenti)
    End Function

    ''' <summary>
    ''' Associa un profilo agli utenti specificati assegnando permessi ed eventualmente impostazioni.
    ''' Se tra i permessi è compresa la gestione della cartografia, inizializza i layer GIS.
    ''' </summary>
    ''' <param name="flagInitTransaction">
    ''' Indica se avviare una transazione per eseguire le operazioni su blocchi di 10_000 utenti.
    ''' </param>
    ''' <param name="avoidAllTransactions">
    ''' Se True, evita di avviare qualunque transazione sul db utenti.
    ''' Opzione attualmente usata nell'importazione di utenti da excel in quanto
    ''' la creazione completa dell'utente avviene già in una transazione unica.
    ''' </param>
    Public Sub AssociaProfilo(
        utenti As IEnumerable(Of IUtente),
        profilo As TipologiaUtente,
        associaImpostazioni As Boolean,
        params As ObjParams,
        Optional flagInitTransaction As Boolean = True,
        Optional avoidAllTransactions As Boolean = False
    )
        Dim objPermesso_W As New AgronicaCoreUtentiDAL.Utenti_Permessi_W
        Dim scriviUtenti As New AgronicaCoreUtentiDAL.Utenti_Write
        Dim scriviImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
        Dim haPermessoCartografia = HaPermessoCartografiaAziendale(profilo, params.ObjParametri_Utenti)
        Dim uu = utenti.AsParallel.Select(Function(u, i) New With {.name = u, .index = i}).
                    GroupBy(Function(u) u.index \ 10_000).
                    Select(Function(batch) batch.Select(Function(ul) ul.name).ToArray). ' lista di usernames 
                    ToList

        uu.ForEach(Sub(u)
                       Try
                           If flagInitTransaction AndAlso Not avoidAllTransactions Then
                               AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(flagInitTransaction, params.ObjParametri_Utenti)
                           End If

                           scriviUtenti.ModificaTipologiaMassivo(u, profilo.codice, params.ObjParametri_Utenti)
                           objPermesso_W.ScriviDaTipologiaMassivo(u, "", params.ObjParametri_Utenti, Not flagInitTransaction AndAlso Not avoidAllTransactions)
                           If associaImpostazioni Then
                               scriviImpostazioni.ScriviDaTipologiaMassivo(u, "", params.ObjParametri_Utenti, Not flagInitTransaction AndAlso Not avoidAllTransactions)
                           End If
                           If haPermessoCartografia Then
                               InitGisLayersFromProfile(u, params.ObjParametri_Utenti, params.ObjParametri_Server)
                           End If

                           If flagInitTransaction AndAlso Not avoidAllTransactions Then
                               AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, params.ObjParametri_Utenti)
                               AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(params.ObjParametri_Utenti)
                           End If
                       Catch ex As Exception
                           If flagInitTransaction AndAlso Not avoidAllTransactions Then
                               AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, params.ObjParametri_Utenti)
                               AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(params.ObjParametri_Utenti)
                           End If
                           Throw
                       End Try
                   End Sub)


        Dim gestoreCache As New AgronicaCoreVarieBIZ.GestoreCache
        Dim client As New HttpClient()

        gestoreCache.PulisciCachePermessi(client, params.ObjParametri_Server)

        If associaImpostazioni Then
            gestoreCache.PulisciCacheImpostazioni(client, params.ObjParametri_Server)
        End If

    End Sub

    Private Function HaPermessoCartografiaAziendale(profilo As TipologiaUtente, objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim objTipologie As New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_R
        Dim filtraCartografiaAziendale = " Id_Attivita = " & enum_Security_Attivita.Gest_CartografiaAziendale & " "
        Return objTipologie.Leggi(
            profilo.codice, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
            filtraCartografiaAziendale, String.Empty, objParametri_Utenti
        ).Rows.Count > 0
    End Function

    ''' <summary>
    ''' Inizializza i layers nel GIS per gi utenti specificati.
    ''' </summary>
    ''' <param name="users">Username dell'utente (o utenti) per cui inizializzare i layers. Se sono previsti più utenti, gli username devono essere contenuti tra apici e concatenati da virgola i.e. "'user1', 'user2', ..."</param>
    Private Sub InitGisLayersFromProfile(utenteDestinazione As String, objParametri_utenti As AgronicaCoreParametri, objParametri_server As AgronicaCoreParametri)
        Dim xGisDalPermessi As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W
        Dim tipologiaLayerCod = 1
        Dim hasUpdated As Boolean = xGisDalPermessi.UpdateSuperUserOneShot(objParametri_server.PivaSuperUser, objParametri_server)
        If Not hasUpdated Then
            Throw New Exception("Cfg Super user non riuscita")
        End If

        Dim isCopyOk As Boolean = xGisDalPermessi.RicopiaLayerDaAltroUtenteTipologiaMultiplo(
            objParametri_server.SuperUserUsername, utenteDestinazione,
            tipologiaLayerCod, objParametri_utenti, objParametri_server
        )
        If Not isCopyOk Then
            Throw New Exception("Copia da utente non riuscita")
        End If

        'Lavez - 20/03/2025 - pezza temporanea per layertiles
        Dim isCopyTilesOk As Boolean = xGisDalPermessi.RicopiaLayerTilesDaAltroUtenteTipologiaMultiplo(
            objParametri_server.SuperUserUsername, utenteDestinazione,
            tipologiaLayerCod, objParametri_utenti, objParametri_server
        )
        If Not isCopyTilesOk Then
            Throw New Exception("Copia da utente non riuscita")
        End If
    End Sub

    ''' <summary>
    ''' Inizializza i layers nel GIS per gi utenti specificati.
    ''' </summary>
    ''' <param name="users">Username dell'utente (o utenti) per cui inizializzare i layers. Se sono previsti più utenti, gli username devono essere contenuti tra apici e concatenati da virgola i.e. "'user1', 'user2', ..."</param>
    Private Sub InitGisLayersFromProfile(utenteDestinazione As IEnumerable(Of IUtente), objParametri_utenti As AgronicaCoreParametri, objParametri_server As AgronicaCoreParametri)
        Dim xGisDalPermessi As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W
        Dim tipologiaLayerCod = 1
        Dim hasUpdated As Boolean = xGisDalPermessi.UpdateSuperUserOneShot(objParametri_server.PivaSuperUser, objParametri_server)
        If Not hasUpdated Then
            Throw New Exception("Cfg Super user non riuscita")
        End If

        Dim isCopyOk As Boolean = xGisDalPermessi.RicopiaLayerDaAltroUtenteTipologiaMultiplo(
            objParametri_server.SuperUserUsername, utenteDestinazione,
            tipologiaLayerCod, objParametri_utenti, objParametri_server
        )
        If Not isCopyOk Then
            Throw New Exception("Copia da utente non riuscita")
        End If
        'Lavez - 20/03/2025 - pezza temporanea per layertiles
        Dim isCopyTilesOk As Boolean = xGisDalPermessi.RicopiaLayerTilesDaAltroUtenteTipologiaMultiplo(
            objParametri_server.SuperUserUsername, utenteDestinazione,
            tipologiaLayerCod, objParametri_utenti, objParametri_server
        )
        If Not isCopyTilesOk Then
            Throw New Exception("Copia da utente non riuscita")
        End If
    End Sub

    Public Sub ModificaValiditaPermessi(utenti As List(Of UtentePermessi), objServer As AgronicaCoreParametri, objUtenti As AgronicaCoreParametri)
        '----- APRO CONNESSIONE AL DATABASE
        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objUtenti)
        utenti.Where(Function(u) usernameUsato(u.UserName, objUtenti)).ToList.
            ForEach(Sub(u) AggiornaPermessi(u, objUtenti, False))
        '----- CHIUDO CONNESSIONE AL DATABASE
        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objUtenti)
        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objUtenti)

        Dim gestoreCache As New AgronicaCoreVarieBIZ.GestoreCache
        Dim client As New HttpClient()

        gestoreCache.PulisciCachePermessi(client, objServer)
        gestoreCache.PulisciCacheImpostazioni(client, objServer)

    End Sub

    Private Sub AssociaImpostazioniProfilo(utente As UtentePermessi, objUtenti As AgronicaCoreParametri)
        Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
        Dim objImpostazioniFiltroMono As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_W
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim DtUtenti As DataTable

        'Verifica desistenza username
        Dim Username = utente.UserName.ToLower()
        DtUtenti = objUtentiDAL.Leggi(
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            "Utenti.UserName = '" & Agro_SQL_SaveText(Username) & "' ", "",
            objUtenti
        )

        If DtUtenti IsNot Nothing AndAlso DtUtenti.Rows.Count > 0 Then
            'Se l'username esiste e quindi sono in modifica, cancello tutto prima di riscrivere
            Dim filtroImpostazioni As String = ""
            Dim filtroMono As String = ""
            Dim impostazioneCod = 0

            Dim objUtentiImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_W
            objUtentiImpostazioni.SetFiltroImpostazioni(filtroImpostazioni, filtroMono)

            objImpostazioni.Cancella2(Username, impostazioneCod, filtroImpostazioni, objUtenti)
            objImpostazioniFiltroMono.Cancella(Username, impostazioneCod, filtroMono, objUtenti)
        End If

        objImpostazioni.ApplicaProfilo(utente.Tipologia.codice, Username, objUtenti)
        objImpostazioniFiltroMono.ApplicaProfilo(utente.Tipologia.codice, Username, objUtenti)
    End Sub

    Public Sub DisattivaPermessiUtenti(usernames As IEnumerable(Of String), objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)
        Dim objTipologiexPermessi As New AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_W
        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Utenti)

        Dim yesterday = Now.AddDays(-1)
        For Each Utente In usernames
            objTipologiexPermessi.AggiornaValiditaPermessiUtente(enum_Id_Servizio.GiasOnline, Utente,
                                                                 yesterday, objParametri_Utenti)
        Next

        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Utenti)
        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Utenti)

        Dim gestoreCache As New AgronicaCoreVarieBIZ.GestoreCache
        Dim client As New HttpClient()

        gestoreCache.PulisciCachePermessi(client, objParametri_Server)
        gestoreCache.PulisciCacheImpostazioni(client, objParametri_Server)
    End Sub

#End Region

    '==========================================================================

    Public Function Carica_Gruppi(objParametri_Utenti As AgronicaCoreParametri) As IEnumerable(Of GruppoUtente)
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Gruppi_Utente_R
        Dim dt = objUtentiDAL.Leggi(0, "", "", objParametri_Utenti)
        Dim listItems = From dr In dt.Rows
                        Select New GruppoUtente() With {
                               .codice = dr.Item("Gruppi_Utente_cod"),
                               .descrizione = dr.Item("Gruppi_Utente_des"),
                               .Identificativo = dr.Item("Gruppi_Utente_Identificativo")
                            }
        Return listItems
    End Function

    Public Function LeggiLingua(ByVal CodiceISO As String, ByRef objParametri_Utenti As AgronicaCoreParametri) As DataTable
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim DT = objUtentiDAL.LeggiLingue(CodiceISO, "", "", objParametri_Utenti)
        Return DT
    End Function

    Public Sub ImpostaLinguaUtente(username As String, linguaCod As Integer, objParametri_Utenti As AgronicaCoreParametri)
        Dim objUtentiWrite As New Utenti_Write
        objUtentiWrite.ModificaLingua(username, linguaCod, objParametri_Utenti)
    End Sub

    ''' <summary>
    ''' Retrieves a random Codice Fiscale (Italian tax code) using sequential generation method.
    ''' </summary>
    ''' <param name="params">An ObjParams object containing server parameters required for Codice Fiscale generation.</param>
    ''' <returns>A String representing the generated Codice Fiscale.</returns>
    Public Function GetRandomCodFisc(params As ObjParams) As String
        Dim userWriter As New AgronicaCoreUtentiDAL.UserWriter
        Return userWriter.GetNextSequentialCF(params.ObjParametri_Server)
    End Function

End Class

Public Class Widgets

    Public Function Leggi(ByVal IdWidget As Integer,
                          ByVal userName As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim objWidgets As New AgronicaCoreUtentiDAL.Utenti_Widgets_R
        Dim dt As DataTable = Nothing
        dt = objWidgets.Leggi(userName,
                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                              "", "",
                              objParametri,,, IdWidget)
        Return dt
    End Function

    Public Function LeggiWidgets(ByVal UserName As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Visibile As Boolean? = Nothing,
                            Optional ByVal Abilitato As Boolean? = Nothing
    ) As List(Of Widget)
        Dim retVal As New List(Of Widget)
        Dim objWidgets As New AgronicaCoreUtentiDAL.Utenti_Widgets_R
        Dim dt As DataTable = Nothing

        If EsisteTabella("Utenti_Widgets", objParametri) Then
            dt = objWidgets.Leggi(UserName, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                "", "", objParametri, Visibile, Abilitato)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                For Each r As DataRow In dt.Rows
                    Dim w As New Widget With
                    {
                        .IdWidget = Convert.ToInt32(r.Item("IdWidget")),
                        .UserName = r.Item("UserName").ToString(),
                        .Abilitato = Convert.ToBoolean(r.Item("Abilitato")),
                        .Visibile = Convert.ToBoolean(r.Item("Visibile")),
                        .Aspetto = IIf(r.Item("Aspetto") Is DBNull.Value, "", r.Item("Aspetto")),
                        .Parametri = IIf(r.Item("Parametri") Is DBNull.Value, "", r.Item("Parametri"))
                    }
                    retVal.Add(w)
                Next
            End If

        End If

        Return retVal
    End Function

    ''' <param name="username">Se non indicato, aggiorna il record per l'utente corrente.</param>
    Public Function AggiornaWidgetUtente(
        ByVal Widget As Widget,
        ByRef objParametri As AgronicaCoreParametri,
        Optional ByVal username As String = ""
    ) As String
        Dim objWidget As New Utenti_Widgets_W
        Dim risposta As String

        If String.IsNullOrEmpty(username) Then
            username = objParametri.UtenteUsername
        End If

        Try
            objWidget.Modifica(
                username, Widget.IdWidget, Widget.Visibile, Widget.Abilitato,
                Widget.Aspetto, Widget.Parametri, objParametri
            )
            risposta = String.Format("OK")
        Catch ex As Exception
            risposta = String.Format("Errore nel salvataggio del widget con Id {0}. Errore: {1}", Widget.IdWidget, ex.Message)
        End Try

        Return risposta
    End Function

    Public Function CancellaWidgets(ByVal userName As String, ByRef objParametri As AgronicaCoreParametri) As String
        Dim objWidget As New Utenti_Widgets_W
        Dim risposta As String
        Try
            objWidget.Cancella(userName, "", objParametri)
            risposta = "OK"

        Catch ex As Exception
            risposta = String.Format("Errore in fase di eliminazione widget per l'utente {0}. Errore: {1}", userName, ex.Message)

        End Try
        Return risposta
    End Function

    Public Function ScriviWidgetUtente(ByVal Widget As Widget, ByRef objParametri As AgronicaCoreParametri) As String
        Dim objWidget As New Utenti_Widgets_W
        Dim risposta As String
        Try
            objWidget.Scrivi(Widget.UserName, Widget.IdWidget, Widget.Visibile, Widget.Abilitato, Widget.Aspetto,
                    Widget.Parametri, objParametri,,, Widget.UserNameCreazione, Widget.UserNameModifica)
            risposta = String.Format("OK")

        Catch ex As Exception
            risposta = String.Format("Errore nel salvataggio del widget con Id {0}. Errore: {1}", Widget.IdWidget, ex.Message)

        End Try
        Return risposta
    End Function

    Public Function ScriviAggiornaWidgetsUtente(ByVal widgets As IEnumerable(Of Widget), ByRef params As ObjParams) As List(Of String)
        Dim objWidget As New AgronicaCoreMetaSchemaDAL.Widgets_R
        Dim risposte As New List(Of String)()

        If IsNothing(widgets) OrElse Not widgets.Any Then
            Return risposte
        End If

        Dim userWidgets = LeggiWidgets(widgets(0).UserName, params.ObjParametri_Utenti)
        Dim metaWidgets = objWidget.Leggi(
            "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta,
            "", "", params.ObjParametri_Server
        ).Select.Select(Function(r) New Widget With {
            .IdWidget = Convert.ToInt32(r.Item("IdWidget")),
            .Abilitato = Convert.ToBoolean(r.Item("Abilitato")),
            .Visibile = Convert.ToBoolean(r.Item("Visibile")),
            .RichiedeAziendaSelezionata = Convert.ToBoolean(r.Item("RichiedeAziendaSelezionata")),
            .Aspetto = IIf(r.Item("Aspetto") Is DBNull.Value, "", r.Item("Aspetto")),
            .Parametri = IIf(r.Item("Parametri") Is DBNull.Value, "", r.Item("Parametri")),
            .Codice = IIf(r.Item("Codice") Is DBNull.Value, "", r.Item("Codice")),
            .Descrizione = IIf(r.Item("Descrizione") Is DBNull.Value, "", r.Item("Descrizione")),
            .Titolo = IIf(r.Item("Titolo") Is DBNull.Value, "", r.Item("Titolo")),
            .TipoWidget = IIf(r.Item("TipoWidget") Is DBNull.Value, "", r.Item("TipoWidget")),
            .PresetIniziale = Convert.ToBoolean(r.Item("PresetIniziale"))
        })

        For Each widget In widgets
            Dim found = userWidgets.FirstOrDefault(Function(w) w.IdWidget = widget.IdWidget)
            If IsNothing(found) Then
                Dim metaWidget = metaWidgets.FirstOrDefault(Function(w) w.IdWidget = widget.IdWidget)
                If Not IsNothing(metaWidget) Then
                    widget.Parametri = metaWidget.Parametri
                    widget.Aspetto = metaWidget.Aspetto
                End If
                risposte.Add(ScriviWidgetUtente(widget, params.ObjParametri_Utenti))
            Else
                risposte.Add(AggiornaWidgetUtente(widget, params.ObjParametri_Utenti, widget.UserName))
            End If
        Next
        Return risposte
    End Function

    Public Function PresetInizialeWidgetUtente(ByVal widgets As List(Of Widget), ByRef objParametri As AgronicaCoreParametri) As List(Of String)
        Dim retVal As New List(Of String)
        For Each w As Widget In widgets
            retVal.Add(ScriviWidgetUtente(w, objParametri))
        Next
        Return retVal
    End Function

    Private Function EsisteTabella(ByVal nomeTabella As String, ByVal objParametri As AgronicaCoreParametri) As Boolean
        Dim retVal As Boolean
        Dim xDal As New ConfigurazioneBase2
        retVal = xDal.EsisteTabellaQry(nomeTabella, objParametri)
        Return retVal
    End Function

End Class