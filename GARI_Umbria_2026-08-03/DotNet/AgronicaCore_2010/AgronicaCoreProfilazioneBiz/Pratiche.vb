Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq
Imports <xmlns="http://G2G">

Public Class Pratiche_ServiziStati

    Public ServizioCod As Integer
    Public StatoCod As Integer
    Public Descrizione As String
    Public Utente As String
    Public Piva As String

End Class

Public Class Pratiche_R


    Public Function LeggiConEF(ByVal piva As String, ByRef objParametri As AgronicaCoreParametri) As String


        Dim NomeRoutine As String = "AnagrafeBIZ.Analisi_Testata_R.Analisi_Testata_LeggiConEF()"

        Dim rval As String


        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        GiasContext.Configuration.LazyLoadingEnabled = False

        Dim o As List(Of Pratiche) = (
            From p In GiasContext.Pratiche.Include("Pratiche_Stati").Include("Pratiche_Stati_Attuali")
            Where p.Piva = piva
            Select p).ToList()

        rval = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of List(Of Pratiche))(o, "")


        Return rval

    End Function

    Public Function StatoAttualeDaPivaServizio(piva As String, cuaa As String, ByVal servizioCod As enum_Servizi_Stati, ByRef objParametri_Server As AgronicaCoreParametri) As Integer

        Dim statoAttuale As Integer = 0

        Dim praticheR As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim dtPratica As DataTable = praticheR.Leggi_2(0, piva, cuaa, 0, 0, 0, servizioCod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, False)

        Dim pratica_cod As Integer = 0
        If dtPratica.Rows.Count > 0 Then
            pratica_cod = dtPratica.Rows(0)("pratica_cod")
        End If

        Dim praticheStatiLeggi As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_R
        Dim xDtoPraticheStati As DataTable =
            praticheStatiLeggi.Leggi(pratica_cod, "", "", objParametri_Server)

        If xDtoPraticheStati.Rows.Count > 0 Then
            statoAttuale = xDtoPraticheStati.Rows(0)("Stato_Cod")
        End If


        Return statoAttuale

    End Function

    Public Function UltimoStatoDaTransizioneDiStatoDaPivaServizio(piva As String, cuaa As String, ByVal servizioCod As enum_Servizi_Stati, ByRef objParametri_Server As AgronicaCoreParametri) As Integer

        Dim praticheR As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim dtPratica As DataTable = praticheR.Leggi_2(0, piva, cuaa, 0, 0, 0, servizioCod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, False)

        Dim pratica_cod As Integer = 0
        If dtPratica.Rows.Count > 0 Then
            pratica_cod = dtPratica.Rows(0)("pratica_cod")
        End If

        Dim praticheStatiLeggi As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_R
        Dim statoAttuale As Integer = StatoAttualeDaPivaServizio(piva, cuaa, servizioCod, objParametri_Server)
        Dim xFiltroPraticheStati As String = " Pratiche_Stati.Servizio_Cod = " & servizioCod
        Dim xOrderByPraticheStati As String = " Pratiche_Stati.Validita_inizio "
        Dim xDtoPraticheStati As DataTable =
            praticheStatiLeggi.Leggi(pratica_cod, statoAttuale, AGRODATAINIZIO, AGRODATAFINE, xFiltroPraticheStati, xOrderByPraticheStati, objParametri_Server, 0)

        Dim ultimoStato As Integer = 0
        If xDtoPraticheStati.Rows.Count > 0 Then
            ultimoStato = xDtoPraticheStati.Rows(0)("Stato_Origine_Cod")
        End If

        Return ultimoStato

    End Function
    Public Function leggiServiziStati(WorkflowCod As Integer, piva As String, servizioCod As Integer, ByVal username As String, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard


        Dim rval As New RispostaStandard
        rval.RispostaOK = True

        Try

            Dim praticheR As New AgronicaCoreProfilazioneDAL.Pratiche_R

            Dim xFiltroAggiuntivo As String = ""
            If username <> "" Then
                xFiltroAggiuntivo = " cfg.Username_Creazione = '" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(username) & "'"
            End If
            Dim DtPratiche As DataTable = praticheR.LeggiXServiziStatoAttuale(WorkflowCod, piva, xFiltroAggiuntivo, "", objParametri_Server, True)


            Dim listaPratiche As New List(Of String)
            Dim praticheAccodate As New List(Of Pratiche_ServiziStati)

            For Each drPratiche In DtPratiche.Rows

                'il workflow di attivazione prevede servizi e stati gerarchici, da gestire a parte.
                If {enum_Servizi.QStandard,
                    enum_Servizi.QPlus, enum_Servizi.QBio,
                    enum_Servizi.QMaps, enum_Servizi.QFert
                   }.Contains(drPratiche("Servizio_Cod")) Then

                    Workflow_di_attivazione_aziende_GIAS(
                        drPratiche("Servizio_Cod"), drPratiche("WAnagraficaStati_Cod"),
                        AgronicaCoreUtility.jSon.Escape(CStr(drPratiche("Servizio_Des"))),
                        AgronicaCoreUtility.jSon.Escape(CStr(drPratiche("WAnagraficaStati_Des"))),
                        drPratiche("piva"), drPratiche("utente").ToUpper(),
                        praticheAccodate)

                Else

                    praticheAccodate.Add(New Pratiche_ServiziStati With {.ServizioCod = 0, .StatoCod = 0, .Descrizione = "{" &
                                        " ""ServizioDes"": """ & AgronicaCoreUtility.jSon.Escape(CStr(drPratiche("Servizio_Des"))) & """," &
                                        " ""StatoDes"": """ & AgronicaCoreUtility.jSon.Escape(CStr(drPratiche("WAnagraficaStati_Des"))) & """," &
                                        " ""ServizioCod"": """ & AgronicaCoreUtility.jSon.Escape(CStr(drPratiche("Servizio_Cod"))) & """," &
                                        " ""Piva"": """ & AgronicaCoreUtility.jSon.Escape(CStr(drPratiche("piva"))) & """," &
                                        " ""Utente"": """ & AgronicaCoreUtility.jSon.Escape(CStr(drPratiche("utente")).ToUpper()) & """" &
                                        "}", .Piva = drPratiche("piva"), .Utente = drPratiche("utente").ToUpper()
                                        }
                                    )
                End If



            Next

            listaPratiche = (From pp In praticheAccodate Select pp.Descrizione).ToList

            rval.RispostaStringa = "[" & String.Join(",", listaPratiche) & "]"


        Catch ex As Exception

            rval.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try


        Return rval


    End Function

    Private Sub Workflow_di_attivazione_aziende_GIAS(ByVal servizioCod As Integer, ByVal statoCod As Integer, ServizioDes As String, statoDes As String, piva As String, utente As String, ByRef listaPratiche As List(Of Pratiche_ServiziStati))

        Dim accoda As Boolean = False

        Select Case servizioCod

            'rimuovo il qstandard ed aggiungo
            Case enum_Servizi.QPlus, enum_Servizi.QBio
                Dim toRemove As List(Of Pratiche_ServiziStati) = (
                    From ii In listaPratiche Where ii.ServizioCod = enum_Servizi.QStandard
                ).ToList

                For Each r In toRemove
                    listaPratiche.Remove(r)
                Next

                accoda = True


            Case Else

                accoda = True

        End Select

        If accoda Then
            listaPratiche.Add(New Pratiche_ServiziStati With {.ServizioCod = servizioCod, .StatoCod = statoCod, .Descrizione =
                "{" &
                " ""ServizioDes"": """ & AgronicaCoreUtility.jSon.Escape(ServizioDes) & """," &
                " ""StatoDes"": """ & AgronicaCoreUtility.jSon.Escape(statoDes) & """," &
                " ""ServizioCod"": """ & AgronicaCoreUtility.jSon.Escape(servizioCod) & """," &
                " ""Piva"": """ & AgronicaCoreUtility.jSon.Escape(piva) & """," &
                " ""Utente"": """ & AgronicaCoreUtility.jSon.Escape(utente) & """" &
                "}", .Piva = piva, .Utente = utente
                }
            )

        End If


    End Sub

    Public Sub Data_Sportello_Da_Servizio(ByVal Piva As String,
                                          ByVal Servizio_Cod As Integer,
                                          ByVal Data_Riferimento As DateTime,
                                          ByRef SportelloAperto As Boolean,
                                          ByRef Data_Inizio As DateTime,
                                          ByRef Data_Fine As DateTime,
                                          objParametri_Server As AgronicaCoreParametri,
                                          objParametri_Utenti As AgronicaCoreParametri)


        'Anche se objParametri_Utenti era gia' passato la maggior parte delle volte veniva passato a nothing, e spesso il chiamante non lo aveva, e dove non c'era e' stato aggiunto
        ' un optional con default value a nothing.
        If Not IsNothing(objParametri_Utenti) Then
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim permessoModificaOperazioneinVerifica As Boolean = objPermessi.Controlla_Permessi_Utente(
                objParametri_Utenti.UtenteUsername,
                enum_Id_Servizio.GiasOnline,
                enum_Security_Attivita.ModificaOperazioneinVerifica,
                enum_Security_Operazione.Modifica,
                Date.Now,
                "",
                objParametri_Utenti
                )

            If permessoModificaOperazioneinVerifica Then
                SportelloAperto = True
                Data_Inizio = AGRODATAINIZIO
                Data_Fine = AGRODATAFINE
                Return
            End If
        End If

        Dim objPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim objServizixSportello As New AgronicaCoreProfilazioneDAL.ServizixSportello_R

        Dim dtPratiche = objPratiche.Leggi_FiltroUtente(0,
                                                        "",
                                                        Piva,
                                                        "",
                                                        0,
                                                        0,
                                                        0,
                                                        Servizio_Cod,
                                                        0,
                                                        AGRODATAINIZIO,
                                                        AGRODATAFINE,
                                                        " Pratiche_Stati_Attuali.Stato_Cod <> 2004 ",
                                                        "",
                                                        False,
                                                        objParametri_Server,
                                                        objParametri_Utenti,
                                                        False)
        SportelloAperto = True

        If dtPratiche.Rows.Count > 0 Then
            'Se ha almeno una pratica di quel servizio verifico lo sportello
            Dim dtServizixSportelli = objServizixSportello.Leggi(Servizio_Cod, 0, "", "", objParametri_Server)
            If dtServizixSportelli.Rows.Count > 0 Then
                Dim DtSportelli = objServizixSportello.LeggiAttivoAllaData(Servizio_Cod, Data_Riferimento, "", "", objParametri_Server)
                If DtSportelli.Rows.Count > 0 Then
                    If DtSportelli.Compute(" MAX(Validita_Fine) ", "") < DateTime.Now Then
                        SportelloAperto = False
                    Else
                        SportelloAperto = True
                    End If
                    Data_Inizio = DtSportelli.Compute(" MIN(Validita_Inizio) ", "")
                    Data_Fine = DtSportelli.Compute(" MAX(Validita_Fine) ", "")
                Else
                    SportelloAperto = False
                End If
            End If
        End If

    End Sub

    Public Sub Limitazione_Data_Per_VerificaInCorso(ByVal Piva As String,
                                          ByVal Servizio_Cod As Integer,
                                          ByVal Data_Riferimento As DateTime,
                                          ByRef AziendaInVerifica As Boolean,
                                          ByRef Data_Inizio As DateTime,
                                          ByRef Data_Fine As DateTime,
                                          objParametri_Server As AgronicaCoreParametri,
                                          objParametri_Utenti As AgronicaCoreParametri)



        'Anche se objParametri_Utenti era gia' passato la maggior parte delle volte veniva passato a nothing, e spesso il chiamante non lo aveva, e dove non c'era e' stato aggiunto
        ' un optional con default value a nothing.
        If Not IsNothing(objParametri_Utenti) Then
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim permessoModificaOperazioneinVerifica As Boolean = objPermessi.Controlla_Permessi_Utente(
                objParametri_Utenti.UtenteUsername,
                enum_Id_Servizio.GiasOnline,
                enum_Security_Attivita.ModificaOperazioneinVerifica,
                enum_Security_Operazione.Modifica,
                Date.Now,
                "",
                objParametri_Utenti
                )

            If permessoModificaOperazioneinVerifica Then
                AziendaInVerifica = False
                Data_Inizio = AGRODATAINIZIO
                Data_Fine = AGRODATAFINE
                Return
            End If
        End If

        Dim objPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim objPraticheStati As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_R
        Dim objServizixSportello As New AgronicaCoreProfilazioneDAL.ServizixSportello_R

        Dim dtPratiche = objPratiche.Leggi_FiltroUtente(0,
                                                        "",
                                                        Piva,
                                                        "",
                                                        0,
                                                        0,
                                                        0,
                                                        Servizio_Cod,
                                                        0,
                                                        Data_Riferimento,
                                                        Data_Riferimento,
                                                        " Pratiche_Stati_Attuali.Stato_Cod <> 2004 ",
                                                        "",
                                                        False,
                                                        objParametri_Server,
                                                        objParametri_Utenti,
                                                        False)
        AziendaInVerifica = False

        If dtPratiche.Rows.Count > 0 Then
            Dim strPratiche = ""
            For Each rowPratica In dtPratiche.Rows
                strPratiche &= rowPratica("Pratica_Cod") & ", "
            Next
            strPratiche = strPratiche.Substring(0, strPratiche.Length - 2)

            Dim dt = objPraticheStati.Leggi(0,
                                   enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_Verifica_in_corso,
                                   AGRODATAINIZIO,
                                   AGRODATAFINE,
                                   " Pratica_Cod IN (" & strPratiche & ") ",
                                   " Validita_Inizio DESC ",
                                    objParametri_Server, 0)

            If dt.Rows.Count > 0 Then
                Data_Inizio = dt.Rows(0)("Validita_Inizio")
                If Data_Inizio > Data_Riferimento Then
                    AziendaInVerifica = True
                End If
            End If

        End If

    End Sub

    Public Sub Data_Sportello_Da_Servizio_Pive_Multiple(ByVal ListaPive As List(Of String),
                                                        ByVal Servizio_Cod As Integer,
                                                        ByVal Data_Riferimento As DateTime,
                                                        ByRef SportelloAperto As Boolean,
                                                        ByRef Data_Inizio As DateTime,
                                                        ByRef Data_Fine As DateTime,
                                                        objParametri_Server As AgronicaCoreParametri,
                                                        objParametri_Utenti As AgronicaCoreParametri)

        Data_Inizio = AGRODATAINIZIO
        Data_Fine = AGRODATAFINE
        SportelloAperto = True

        For Each piva In ListaPive
            Dim SportelloApertoInterno As Boolean = True
            Dim Data_Inizio_Interno = AGRODATAINIZIO
            Dim Data_Fine_Interno = AGRODATAFINE

            Data_Sportello_Da_Servizio(piva, Servizio_Cod, Data_Riferimento, SportelloApertoInterno, Data_Inizio_Interno, Data_Fine_Interno, objParametri_Server, objParametri_Utenti)

            If SportelloApertoInterno = False Then
                SportelloAperto = False
            End If

            If Data_Inizio_Interno > Data_Inizio Then
                Data_Inizio = Data_Inizio_Interno
            End If

            If Data_Fine_Interno < Data_Fine Then
                Data_Fine = Data_Fine_Interno
            End If

        Next

    End Sub

    Public Function LeggiStatoAttualePratica(ByVal praticaCod As Integer,
                                             ByRef objParametri_Server As AgronicaCoreParametri
                                             ) As Integer

        Dim statoCod As Integer = 0

        Dim objPraticaStatiAtt As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_R

        Dim dtStatiAtt = objPraticaStatiAtt.Leggi(praticaCod, "", "", objParametri_Server)

        If Not IsNothing(dtStatiAtt) AndAlso dtStatiAtt.Rows.Count > 0 Then

            statoCod = dtStatiAtt.Rows(0).Item("Stato_Cod")

        End If

        Return statoCod

    End Function

    Public Function VerificaEsistenzaServizioByPivaServizio(ByVal Piva As String,
                                                            ByVal Servizio_Cod As Integer,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByRef objParametri_server As AgronicaCoreParametri) As Boolean

        Dim xRead As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim DT As DataTable

        DT = xRead.Leggi(0, "", Piva, "", 0, 0, 0, Servizio_Cod, AGRODATAINIZIO, AGRODATAFINE, xFiltroAggiuntivo, "", objParametri_server, 0, 0)

        If DT Is Nothing OrElse DT.Rows.Count = 0 Then
            Return False
        End If

        Return True

    End Function

    ''' <summary>
    ''' Esegue una chiamata secondaria alla funzione di verifica solo se non sono stati rilevati cambiamenti nei dati.
    ''' Viene utilizzata dopo la chiamata principale per verificare un servizio secondario. Creata per controllare QuadernoCampagnaBio se dopo il controllo su Quaderno_Campagna_Caa non ci sono stati cambiamenti
    ''' </summary>
    ''' <param name="piva">Partita IVA dell'azienda</param>
    ''' <param name="servizio_Secondario">Codice del servizio secondario da verificare</param>
    ''' <param name="data_riferimeto">Data di riferimento</param>
    ''' <param name="vecchiaAziendaInVerifica">Stato precedente dell'azienda in verifica</param>
    ''' <param name="vecchiaDataMin">Data minima precedente</param>
    ''' <param name="vecchiaDataMax">Data massima precedente</param>
    ''' <param name="aziendaInVerifica">Stato attuale dell'azienda in verifica</param>
    ''' <param name="dataMin">Data minima attuale</param>
    ''' <param name="dataMax">Data massima attuale</param>
    ''' <param name="objParametri_Server">Parametri del server</param>
    ''' <param name="objParametri_Utenti">Parametri degli utenti</param>
    Public Function VerificaInCorso_ChiamataSecondaria_SeNessunCambiamento(
        ByVal piva As String,
        ByVal servizio_Secondario As Integer,
        ByVal data_riferimeto As Date,
        ByVal vecchiaAziendaInVerifica As Boolean,
        ByVal vecchiaDataMin As Date,
        ByVal vecchiaDataMax As Date,
        ByRef aziendaInVerifica As Boolean,
        ByRef dataMin As Date,
        ByRef dataMax As Date,
        ByVal objParametri_Server As Object,
        ByVal objParametri_Utenti As Object) As Boolean


        If vecchiaDataMin = dataMin AndAlso vecchiaDataMax = dataMax AndAlso
        vecchiaAziendaInVerifica = aziendaInVerifica Then
            Limitazione_Data_Per_VerificaInCorso(piva, servizio_Secondario,
            data_riferimeto, aziendaInVerifica, dataMin, dataMax,
            objParametri_Server, objParametri_Utenti)
        End If

        Return True
    End Function

    ''' <summary>
    ''' Esegue una chiamata secondaria alla funzione di controllo sportello solo se non sono stati rilevati cambiamenti nei dati.
    ''' Viene utilizzata dopo la chiamata principale per verificare un servizio secondario. Creata per controllare QuadernoCampagnaBio se dopo il controllo su Quaderno_Campagna_Caa non ci sono stati cambiamenti
    ''' </summary>
    ''' <param name="piva">Partita IVA dell'azienda</param>
    ''' <param name="servizio_Secondario">Codice del servizio secondario da verificare</param>
    ''' <param name="Data_Riferimento">Data di riferimento</param>
    ''' <param name="vecchioSportelloAperto">Stato precedente dello sportello</param>
    ''' <param name="vecchiaDataMin">Data minima precedente</param>
    ''' <param name="vecchiaDataMax">Data massima precedente</param>
    ''' <param name="sportelloAperto">Stato attuale dello sportello</param>
    ''' <param name="dataMin">Data minima attuale</param>
    ''' <param name="dataMax">Data massima attuale</param>
    ''' <param name="objParametri_Server">Parametri del server</param>
    ''' <param name="objParametri_Utenti">Parametri degli utenti</param>
    Public Function Sportello_ChiamataSecondaria_SeNessunCambiamento(
        ByVal piva As String,
        ByVal servizio_Secondario As Integer,
        ByVal Data_Riferimento As Date,
        ByVal vecchioSportelloAperto As Boolean,
        ByVal vecchiaDataMin As Date,
        ByVal vecchiaDataMax As Date,
        ByRef sportelloAperto As Boolean,
        ByRef dataMin As Date,
        ByRef dataMax As Date,
        ByVal objParametri_Server As Object,
        ByVal objParametri_Utenti As Object) As Boolean


        If vecchiaDataMin = dataMin AndAlso vecchiaDataMax = dataMax AndAlso
        vecchioSportelloAperto = sportelloAperto Then
            Data_Sportello_Da_Servizio(piva, servizio_Secondario, Data_Riferimento,
            sportelloAperto, dataMin, dataMax,
            objParametri_Server, objParametri_Utenti)
        End If

        Return True
    End Function
End Class

Public Class Pratiche_W

    Private Const _UpperBoundTabelle As Integer = 2000000000

    Public Function ScriviPerEF(
        Str_XML_Privato As String,
        objParametri_Server As AgronicaCoreParametri,
        objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard

        Dim praticheLista As List(Of Pratiche) = AgronicaCoreUtility.XMLUtility.DeserializzaOggetto(Of List(Of Pratiche))(Str_XML_Privato, "")


    End Function

    Public Function impostaPratica(wWorkflowCod As Integer,
                                   piva As String,
                                   cuaa As String,
                                   user As String,
                                   servizioCod As Integer,
                                   statoFinaleRichiesto As Integer,
                                   objParametri_Server As AgronicaCoreParametri,
                                   objParametri_Utenti As AgronicaCoreParametri,
                                   Anno As Integer,
                                   Numero_Pratica As String,
                                   ByRef Pratica_Cod As Integer,
                                   Pratica_New As Boolean,
                                   ByVal statoFinaleRichiestoAnnotazioni As String,
                                   ByVal Data_Inizio As Date,
                                   ByVal Data_Fine As Date,
                                   ByVal Programmazione_Cod As Integer,
                                   ByVal Programmazione_Entita_Cod As Integer,
                                   Optional ByVal Blocco_Flag As Integer = 0
                                   ) As RispostaStandard


        Dim rval As New RispostaStandard
        rval.RispostaOK = True

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        'Dim strErr As String = ""

        Try

            'Se la connessione è chiusa la apro; se la transazione è chiusa la inizio
            Utility.VerificaApriTransazione(objParametri_Server, FlagConnessioneLocale, FlagTransazioneLocale)

            Dim praticheR As New AgronicaCoreProfilazioneDAL.Pratiche_R
            Dim praticheW As New AgronicaCoreProfilazioneDAL.Pratiche_W

            Dim Gruppo_Utente_Cod As Integer
            Dim Utente_xGruppi_R As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
            Dim Gruppi_Utente_R As New AgronicaCoreUtentiDAL.Gruppi_Utente_R
            Dim Configurazione_Gruppo_str As String = ""

            Dim dt_Gruppo = Utente_xGruppi_R.Leggi(objParametri_Server.UtenteUsername, 0, "", "", objParametri_Utenti)
            If dt_Gruppo IsNot Nothing AndAlso dt_Gruppo.Rows.Count > 0 Then
                Gruppo_Utente_Cod = dt_Gruppo.Rows(0)("Gruppi_Utente_Cod")

                Dim dtGruppo_Utente = Gruppi_Utente_R.Leggi(Gruppo_Utente_Cod, "", "", objParametri_Utenti)
                If dtGruppo_Utente.Rows.Count > 0 AndAlso Not IsDBNull(dtGruppo_Utente.Rows(0)("ConfigurazioniAggiuntive")) AndAlso dtGruppo_Utente.Rows(0)("ConfigurazioniAggiuntive") <> "" Then
                    Configurazione_Gruppo_str = dtGruppo_Utente.Rows(0)("ConfigurazioniAggiuntive")
                End If

            End If

            Dim dtPratica As DataTable = praticheR.Leggi_2(Pratica_Cod, piva, cuaa, 0, 0, 0, servizioCod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, Pratica_New:=Pratica_New)

            Dim esitoTransizioneStatoOk As Boolean = False
            Dim statoIniziale As Integer = 0

            'Dim pratica_cod As Integer

            If dtPratica.Rows.Count = 0 Then

                'Pratica non ancora inserita
                esitoTransizioneStatoOk = ControllaTransizionediStatoIniziale(wWorkflowCod, statoFinaleRichiesto)

                Dim Blocco_Data = AGRODATAINIZIO
                Dim Blocco_Username = ""

                If Blocco_Flag = -1 Then
                    Blocco_Data = DateTime.Now
                    Blocco_Username = objParametri_Server.UtenteUsername
                End If

                If esitoTransizioneStatoOk Then
                    agroProgressivo("Pratiche", objParametri_Server, Pratica_Cod)
                    praticheW.Scrivi(Pratica_Cod, cuaa & " - " & servizioCod,
                                     piva, cuaa, 0, 0, 0, servizioCod, Data_Inizio, Data_Fine,
                                     objParametri_Server, DateTime.Now, DateTime.Now,
                                     objParametri_Server.UsernameOperazione, objParametri_Server.UsernameOperazione,
                                     Anno:=Anno, Numero_Pratica:=Numero_Pratica,
                                     Blocco_Flag:=Blocco_Flag, Blocco_Data:=Blocco_Data, Blocco_Username:=Blocco_Username,
                                     Programmazione_Cod:=Programmazione_Cod,
                                     Programmazione_Entita_Cod:=Programmazione_Entita_Cod)



                    'praticheStati.Scrivi()
                Else
                    rval.RispostaStringa = "Stato non valido"
                    rval.Errore = "Stato non valido"
                    rval.RispostaOK = False
                End If
            Else

                Pratica_Cod = dtPratica.Rows(0)("pratica_Cod")

                'recupero lo stato iniziale in cui si trova la pratica
                Dim praticheStatiLeggi As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_R
                Dim xDtoPraticheStati As DataTable =
                    praticheStatiLeggi.Leggi(Pratica_Cod, "", "", objParametri_Server)

                statoIniziale = xDtoPraticheStati.Rows(0)("Stato_Cod")

                esitoTransizioneStatoOk = ControllaTransizionediStato(wWorkflowCod, servizioCod, statoIniziale, statoFinaleRichiesto, objParametri_Server)

            End If


            If esitoTransizioneStatoOk Then

                Dim praticheStatiAttualiScrivi As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_W
                Dim praticheStatiScrivi As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_W

                'scrivo stato attuale
                If dtPratica.Rows.Count = 0 Then
                    praticheStatiAttualiScrivi.Scrivi(Pratica_Cod, statoFinaleRichiesto, statoFinaleRichiestoAnnotazioni, Now(), AGRODATAFINE, objParametri_Server, #2/1/1900#, #2/1/1900#, "", "")
                Else
                    praticheStatiAttualiScrivi.Modifica(Pratica_Cod, statoFinaleRichiesto, statoIniziale, statoFinaleRichiestoAnnotazioni, Now(), AGRODATAFINE, "", objParametri_Server)
                End If

                'scrivo transizione di stato.    
                Dim PassaggioDiStato_cod As Integer
                agroProgressivo("passaggiodistato_cod", objParametri_Server, PassaggioDiStato_cod)
                praticheStatiScrivi.Scrivi(Pratica_Cod, statoFinaleRichiesto, servizioCod, Now(), AGRODATAFINE, statoFinaleRichiestoAnnotazioni, statoIniziale, PassaggioDiStato_cod, objParametri_Server, #2/1/1900#, #2/1/1900#, "", "")

                If Configurazione_Gruppo_str <> "" Then
                    Dim Configurazione_Gruppo = JObject.Parse(Configurazione_Gruppo_str)
                    If Configurazione_Gruppo("ConfigurazioneG2G_Local") IsNot Nothing AndAlso Configurazione_Gruppo("ConfigurazioneG2G_Local").HasValues Then
                        Dim ConfigurazioneG2G_Local = JArray.Parse(Configurazione_Gruppo("ConfigurazioneG2G_Local").ToString)
                        Dim Servizio = From p In ConfigurazioneG2G_Local Where CInt(p("Servizio_Cod")) = servizioCod Select p
                        If Servizio.Count > 0 Then
                            Dim StatoAttuale_Cod As Integer = statoIniziale
                            Dim Transizioni_Arr = JArray.Parse(Servizio(0)("Transizioni").ToString)
                            Dim G2GLocalConfigurazioni_COD = CInt(Servizio(0)("G2GLocalConfigurazioni_COD"))
                            Dim Transizione = From p In Transizioni_Arr Where CInt(p("Stato_A")) = statoFinaleRichiesto AndAlso CInt(p("Stato_Da")) = StatoAttuale_Cod Select p
                            If Transizione.Count > 0 Then
                                InserisciAziendaG2G(G2GLocalConfigurazioni_COD, piva, "", "00000000000", objParametri_Server, objParametri_Utenti)
                            End If
                        End If
                    End If
                End If

            Else

                rval.Errore = "Stato non valido"
                rval.RispostaStringa = "Stato non valido"
                rval.RispostaOK = False
                rval.ParametroDue_stringa = CStr(Pratica_Cod)

            End If

            rval.ParametroDue_stringa = CStr(Pratica_Cod)

            Utility.VerificaChiudiTransazione(objParametri_Server, FlagTransazioneLocale)

            ' Sostituzione del trigger tr_up_cascade_update (ramo INSERT): se e' stata creata una
            ' nuova pratica, invalido la cache di visibilita' appoggio degli utenti con filtro
            ' pratiche sul relativo servizio.
            If Pratica_New AndAlso objParametri_Utenti IsNot Nothing AndAlso servizioCod > 0 Then
                Dim objUpp As New AgronicaCoreUtentiDAL.Utenti_Profili_Pratiche
                objUpp.InvalidaVisibilitaAppoggioXServizio(servizioCod, objParametri_Utenti)
            End If

        Catch ex As Exception

            'rollback della transazione
            Utility.VerificaAnnullaTransazione(objParametri_Server, FlagTransazioneLocale)

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            rval.RispostaOK = False
            rval.Errore = "Errore durante il salvataggio dei dati."

        Finally
            Utility.VerificaChiudiConnessione(objParametri_Server, FlagConnessioneLocale)

        End Try

        Return rval

    End Function

    Public Shared Sub InserisciAziendaG2G(G2GLocalConfigurazioni_COD As Integer, Piva As String, Rag_Soc As String, Padre As String, ObjParametri_Server As AgronicaCoreParametri, ObjParametri_Utenti As AgronicaCoreParametri)
        Dim G2GLocal_R As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim G2GLocal_W As New AgronicaCoreProfilazioneDAL.Pratiche_W

        Dim dt_G2G = G2GLocal_R.LeggiConfigurazioni(G2GLocalConfigurazioni_COD, ObjParametri_Server)

        If dt_G2G.Rows.Count > 0 Then
            Dim configurazione = CStr(dt_G2G.Rows(0)("G2GLocalConfigurazioni_CFG"))
            Dim des = CStr(dt_G2G.Rows(0)("G2GLocalConfigurazioni_DES"))
            Dim xml = XDocument.Parse(configurazione)
            Dim xElemCfg As XElement = xml.<dati>.First
            Dim imprese = xElemCfg.<imprese>.First
            Dim impresa = imprese.<filtronerisultato_azienda>.FirstOrDefault.Value
            If String.IsNullOrEmpty(impresa) Then
                impresa = "{ ""tipo"": ""azienda"", ""chiavi"": [] }"
            End If
            Dim objFiltrone = JObject.Parse(impresa)
            Dim jAziende = JArray.Parse(objFiltrone("chiavi").ToString)
            Dim esisteImpresa = (From p In jAziende Where p = Piva).ToArray.Count
            If esisteImpresa = 0 Then
                jAziende.Add(Piva)
                objFiltrone("chiavi") = jAziende
                'Dim result = XDocUtils.IniettaSottoAlberoDaStringaXml(configurazione, impresaEl.ToString, "//*[local-name()='dati']/*[local-name()='imprese']", "")
                xml.<dati>.First.<imprese>.FirstOrDefault.<filtronerisultato_azienda>.First.Value = objFiltrone.ToString
                For Each node In xml.Root.Descendants
                    If node.Name.NamespaceName = "" Then
                        node.Attributes("xmlns").Remove
                        node.Name = node.Parent.Name.Namespace + node.Name.LocalName
                    End If
                Next
                G2GLocal_W.ModificaG2G(G2GLocalConfigurazioni_COD, des, xml.ToString, "", ObjParametri_Server)
            End If
        End If
    End Sub

    Private Function ControllaTransizionediStatoIniziale(WorkFlow_Cod As Integer, StatoF As Integer) As Boolean

        'al momento sempre valida
        Return True

    End Function

    Private Function ControllaTransizionediStato(WorkFlow_Cod As Integer, servizioCod As Integer, StatoI As Integer, StatoF As Integer, objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim praticheR As New AgronicaCoreProfilazioneDAL.Pratiche_R
        If praticheR.Leggi_TransizioniDisponibiliDatoStato(StatoI, servizioCod, "", "", objParametri_Server, AGRODATAINIZIO, AGRODATAFINE).Select(" WAnagraficaStati_Cod=" & StatoF & " ").Count = 0 Then
            Return False
        Else
            Return True
        End If

    End Function

    Private Shared Sub agroProgressivo(ByVal tabella As String, ByRef objParametri_Server As AgronicaCoreParametri, ByRef pratica_cod As Integer)
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        pratica_cod = objSequenze.NuovoId_Tabella(tabella, 0, _UpperBoundTabelle, objParametri_Server)
    End Sub

    Private Shared Sub agroProgressivo_EF(ByRef EFContext As Gias_DeveloperServer_Entities, ByVal tabella As String, ByRef objParametri_Server As AgronicaCoreParametri, ByRef pratica_cod As Integer)
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        pratica_cod = objSequenze.NuovoId_Tabella_EF(EFContext, tabella, 0, _UpperBoundTabelle, objParametri_Server)
    End Sub

    Public Function UndoPratica(Pratica_Cod As Integer,
                                ByRef objParametri_Server As AgronicaCoreParametri,
                                ByRef objParametri_Utenti As AgronicaCoreParametri,
                                ByRef messaggio As String) As Boolean

        Dim done = False

        Dim objPratiche_R As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim objPratiche_W As New AgronicaCoreProfilazioneDAL.Pratiche_W

        Dim objPratiche_Stati_R As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_R
        Dim objPratiche_Stati_W As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_W

        Dim objPratiche_Stati_Attuali_R As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_R
        Dim objPratiche_Stati_Attuali_W As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_W

        Dim Gruppo_Utente_Cod As Integer
        Dim gruppo_Utente_R As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

        Dim dt_Gruppo = gruppo_Utente_R.Leggi(objParametri_Server.UtenteUsername, 0, "", "", objParametri_Utenti)
        If dt_Gruppo IsNot Nothing AndAlso dt_Gruppo.Rows.Count > 0 Then
            Gruppo_Utente_Cod = dt_Gruppo.Rows(0)("Gruppi_Utente_Cod")
        End If

        Dim dtPassaggiDiStato = objPratiche_Stati_R.Leggi(Pratica_Cod, 0, AGRODATAINIZIO, AGRODATAFINE, "", " Validita_Inizio DESC, PassaggioDiStato_Cod DESC ", objParametri_Server, 0)

        If dtPassaggiDiStato.Rows.Count > 1 Then

            Dim passaggioEl = dtPassaggiDiStato.Rows(0)("PassaggioDiStato_Cod")
            Dim StatoxRicerca = dtPassaggiDiStato.Rows(0)("Stato_Cod")
            Dim StatoOld = dtPassaggiDiStato.Rows(1)("Stato_Cod")
            Dim Validita_Inizio_Old = dtPassaggiDiStato.Rows(1)("Validita_Inizio")

            Dim dtPratica = objPratiche_R.Leggi(Pratica_Cod, "", "", "", 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, 0, 0)

            Dim Servizio_Cod = dtPratica.Rows(0)("Servizio_Cod")

            Dim PassaggioStato_Eliminabile = False

            Dim LeggiStatiPerPassaggio As New AgronicaCoreProfilazioneDAL.Pratiche_R
            Dim rval As DataTable

            rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStatoxGruppoUtente(
                        0,
                        Servizio_Cod,
                        Gruppo_Utente_Cod,
                        "",
                        "",
                        objParametri_Server,
                        objParametri_Utenti,
                        AGRODATAINIZIO,
                        AGRODATAFINE
                        )
            If rval.Rows.Count > 0 Then
                rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStatoxGruppoUtente(
                        StatoOld,
                        Servizio_Cod,
                        Gruppo_Utente_Cod,
                        "",
                        "",
                        objParametri_Server,
                        objParametri_Utenti,
                        AGRODATAINIZIO,
                        AGRODATAFINE
                        )
            Else
                rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStato(
                        StatoOld,
                        Servizio_Cod,
                        "",
                        "",
                        objParametri_Server,
                        AGRODATAINIZIO,
                        AGRODATAFINE
                        )
            End If

            If objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername Then
                rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStato(
                                                                                    StatoOld,
                                                                                    Servizio_Cod,
                                                                                    "",
                                                                                    "",
                                                                                    objParametri_Server,
                                                                                    AGRODATAINIZIO,
                                                                                    AGRODATAFINE
                                                                                    )
            End If

            If rval.Select(" WAnagraficaStati_Cod = " & StatoxRicerca & " ").Length > 0 Then
                PassaggioStato_Eliminabile = True
            ElseIf objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername Then
                PassaggioStato_Eliminabile = True
            Else
                messaggio = "Il Gruppo Utente non ha il permesso di eliminare questo passaggio di stato"
            End If

            '----------------------------------------------------------------------------------------------------
            'Controlli UMA
            '----------------------------------------------------------------------------------------------------
            If PassaggioStato_Eliminabile AndAlso Servizio_Cod = enum_Servizi.Gestione_UMA AndAlso StatoxRicerca = enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo Then
                Dim objUMA_R As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
                Dim dtRichiestaUma = objUMA_R.Leggi("", 0, Pratica_Cod, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
                If dtRichiestaUma.Rows.Count > 0 Then
                    'Controllo rimanenze riassegnabili e recupero accise
                    PassaggioStato_Eliminabile = Controlla_RimanenzeRiassegnabili_RecuperAccise(dtRichiestaUma)
                    'Controllo trasferimenti/restituzioni
                    If PassaggioStato_Eliminabile Then
                        If objUMA_R.EsistonoTrasferimentiRestituzioni(dtRichiestaUma(0).Item("Piva"),
                                                                      dtRichiestaUma(0).Item("Richiesta_Cod"),
                                                                      AGRODATAINIZIO,
                                                                      AGRODATAFINE,
                                                                      objParametri_Server) Then
                            PassaggioStato_Eliminabile = False
                        End If
                    End If
                    'Messaggio
                    If Not PassaggioStato_Eliminabile Then
                        messaggio = "Non è possibile eliminare questo passaggio di stato: pratica legata a richiesta di carburante UMA con gestione rimanenze"
                    End If
                End If
            End If
            '----------------------------------------------------------------------------------------------------

            If PassaggioStato_Eliminabile Then

                objPratiche_Stati_W.Cancella(Pratica_Cod, 0, " PassaggioDiStato_Cod = " & passaggioEl, objParametri_Server)
                objPratiche_Stati_Attuali_W.Modifica(Pratica_Cod, StatoOld, StatoxRicerca, "", Validita_Inizio_Old, AGRODATAFINE, "", objParametri_Server)
                done = True
                messaggio = "Passaggio di stato correttamente eliminato"

            End If

        Else

            messaggio = "Non è possibile eliminare il passaggio di stato iniziale. Eliminare la pratica."

        End If

        Return done

    End Function

    Private Shared Function Controlla_RimanenzeRiassegnabili_RecuperAccise(dtRichiestaUma As DataTable) As Boolean

        Dim elencoColonneDaControllare As New List(Of String)(
            {
            "Rim_Riass_Gasolio",
            "Rim_Riass_Benzina",
            "Rim_Riass_Gasolio_Serra",
            "Rim_Riass_Conf_Gasolio",
            "Rim_Riass_Conf_Benzina",
            "Rim_Riass_Conf_Gasolio_Serra",
            "Rec_Acc_Dich_Gasolio",
            "Rec_Acc_Dich_Benzina",
            "Rec_Acc_Dich_Gasolio_Serra",
            "Rec_Acc_Conf_Gasolio",
            "Rec_Acc_Conf_Benzina",
            "Rec_Acc_Conf_Gasolio_Serra"
            }
            )

        For Each colonna In elencoColonneDaControllare
            Dim valoreColonna = IIf(IsDBNull(dtRichiestaUma(0).Item(colonna)), 0, dtRichiestaUma(0).Item(colonna))
            If valoreColonna > 0 Then
                Return False
            End If
        Next

        Return True

    End Function

    Public Function Esegui_PassaggioDiStato_SuperUser_SenzaVerifiche(ByRef EFContext As Gias_DeveloperServer_Entities,
                                                                     Piva As String,
                                                                     Pratica_Cod As String,
                                                                     servizio_cod As Integer,
                                                                     statoFinaleRichiesto As Integer,
                                                                     Note As String,
                                                                     objParametri_Server As AgronicaCoreParametri)
        Dim esitoTransizioneStatoOk As Boolean = False


        Dim praticheR As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim praticheW As New AgronicaCoreProfilazioneDAL.Pratiche_W


        Dim dtPratica As DataTable = praticheR.Leggi_2(Pratica_Cod, Piva, "", 0, 0, 0, servizio_cod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, False)

        If dtPratica.Rows.Count > 0 Then
            Pratica_Cod = dtPratica.Rows(0)("pratica_Cod")

            'recupero lo stato iniziale in cui si trova la pratica
            Dim praticheStatiLeggi As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_R
            Dim xDtoPraticheStati As DataTable =
                        praticheStatiLeggi.Leggi(Pratica_Cod, "", "", objParametri_Server)

            Dim statoIniziale = xDtoPraticheStati.Rows(0)("Stato_Cod")


            Dim praticheStatiAttualiScrivi As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_W
            Dim praticheStatiScrivi As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_W

            'scrivo stato attuale
            If dtPratica.Rows.Count = 0 Then
                praticheStatiAttualiScrivi.Scrivi(Pratica_Cod, statoFinaleRichiesto, Note, Now(), AGRODATAFINE, objParametri_Server, #2/1/1900#, #2/1/1900#, "", "")
            Else
                praticheStatiAttualiScrivi.Modifica(Pratica_Cod, statoFinaleRichiesto, statoIniziale, "", Now(), AGRODATAFINE, "", objParametri_Server)
            End If

            'scrivo transizione di stato.    
            Dim PassaggioDiStato_cod As Integer
            agroProgressivo_EF(EFContext, "passaggiodistato_cod", objParametri_Server, PassaggioDiStato_cod)
            praticheStatiScrivi.Scrivi(Pratica_Cod, statoFinaleRichiesto, servizio_cod, Now(), AGRODATAFINE, Note, statoIniziale, PassaggioDiStato_cod, objParametri_Server, #2/1/1900#, #2/1/1900#, "", "")

            esitoTransizioneStatoOk = True

        End If




        Return esitoTransizioneStatoOk

    End Function

    ''' <param name="objParametri_Utenti">
    ''' Opzionale. Se valorizzato, dopo la cancellazione invalida la cache di visibilita' appoggio
    ''' degli utenti con filtro pratiche sul servizio della pratica (sostituisce il trigger
    ''' tr_up_cascade_update). Se Nothing l'invalidazione viene saltata (la cache si riallinea
    ''' comunque al login interattivo).
    ''' </param>
    Public Function Elimina_Pratica(Pratica_Cod As Integer, objParametri_Server As AgronicaCoreParametri, ByRef Messaggio_Errore As String,
                                    Optional objParametri_Utenti As AgronicaCoreParametri = Nothing) As Boolean
        Dim EliminataPratica = False
        Messaggio_Errore = ""
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Try

            Dim objUMA_R As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R

            If objUMA_R.Leggi("", 0, Pratica_Cod, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server).Rows.Count > 0 Then

                EliminataPratica = False
                Throw New Exception("Pratica legata ad una richiesta di carburante UMA")

            End If

            Dim objPratica_W As New AgronicaCoreProfilazioneDAL.Pratiche_W
            Dim objPraticaStati_W As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_W
            Dim objPraticaStati_Attuali_W As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_W

            ' Recupero il Servizio_Cod PRIMA della cancellazione (serve per invalidare la cache visibilita')
            Dim servizioCodPratica As Integer = 0
            If objParametri_Utenti IsNot Nothing Then
                Dim objPratica_R As New AgronicaCoreProfilazioneDAL.Pratiche_R
                servizioCodPratica = objPratica_R.Leggi_Servizio_DaPratica(Pratica_Cod, "", "", objParametri_Server)
            End If

            Utility.VerificaApriTransazione(objParametri_Server, FlagConnessioneLocale, FlagTransazioneLocale)

            EliminataPratica = objPratica_W.Cancella(Pratica_Cod, "", objParametri_Server)
            If EliminataPratica = True Then
                objPraticaStati_W.Cancella(Pratica_Cod, 0, "", objParametri_Server)
                objPraticaStati_Attuali_W.Cancella(Pratica_Cod, "", objParametri_Server)
            End If

            Utility.VerificaChiudiTransazione(objParametri_Server, FlagTransazioneLocale)

            ' Sostituzione del trigger tr_up_cascade_update (ramo DELETE): invalido la cache di
            ' visibilita' appoggio degli utenti con filtro pratiche sul servizio della pratica eliminata.
            If EliminataPratica AndAlso objParametri_Utenti IsNot Nothing AndAlso servizioCodPratica > 0 Then
                Dim objUpp As New AgronicaCoreUtentiDAL.Utenti_Profili_Pratiche
                objUpp.InvalidaVisibilitaAppoggioXServizio(servizioCodPratica, objParametri_Utenti)
            End If

        Catch ex As Exception

            EliminataPratica = False
            Messaggio_Errore = ex.Message
            Utility.VerificaAnnullaTransazione(objParametri_Server, FlagTransazioneLocale)

        Finally

            Utility.VerificaChiudiConnessione(objParametri_Server, FlagConnessioneLocale)

        End Try

        Return EliminataPratica

    End Function

    Public Function GeneraPratica(ByVal piva As String,
                                  ByVal Servizio_Cod As Integer,
                                  ByVal Data_Inizio_str As String,
                                  ByVal Data_Fine_str As String,
                                  ByVal Data_Inizio_Pratica_str As String,
                                  ByVal Numero As String,
                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                  ByRef objParametri_Utenti As AgronicaCoreParametri,
                                  Optional ByVal ControllaEsistenzaPratica As Boolean = True,
                                  Optional ByRef Pratica_Cod_Inserita As Integer = 0,
                                  Optional ByVal Blocco_Flag As Integer = 0
                                  ) As RispostaStandard

        Dim r As New RispostaStandard

        Pratica_Cod_Inserita = 0

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            Utility.VerificaApriTransazione(objParametri_Server, FlagConnessioneLocale, FlagTransazioneLocale)

            Dim Data_Inizio As Date = AGRODATAINIZIO
            If Data_Inizio_str <> "" Then
                Data_Inizio = CDate(Data_Inizio_str)
            End If

            Dim Data_Fine As Date = AGRODATAFINE
            If Data_Fine_str <> "" Then
                Data_Fine = CDate(Data_Fine_str)
            End If

            Dim Data_Inizio_Pratica As Date = CDate(Data_Inizio_Pratica_str)

            Dim Cuaa = ""

            Dim objPratiche_R As New AgronicaCoreProfilazioneDAL.Pratiche_R
            Dim dt_Pratiche = objPratiche_R.Leggi(0,
                                                  "",
                                                  piva,
                                                  "",
                                                  0,
                                                  0,
                                                  0,
                                                  Servizio_Cod,
                                                  Data_Inizio,
                                                  Data_Fine,
                                                  "",
                                                  "",
                                                  objParametri_Server,
                                                  0,
                                                  0)

            If ControllaEsistenzaPratica AndAlso dt_Pratiche.Rows.Count > 0 Then
                r.RispostaOK = True
                r.RispostaConferma = False
                r.RispostaStringa = "Non è stato possibile attivare il servizio in quanto esiste già un servizio attivo per l'azienda in queste date."
                Return r
            End If

            Dim InseritaPratica As Boolean = False
            Dim InserisciPratica As Boolean = False

            Dim objPratica_W As New AgronicaCoreProfilazioneDAL.Pratiche_W
            Dim objPraticaStati_W As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_W
            Dim objPraticaStati_Attuale_W As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_W
            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

            Cuaa = objImprese.Leggi_CUAA(piva, objParametri_Server)

            Dim objServizi As New Servizi_R
            Dim Servizio_Des = objServizi.Leggi(Servizio_Cod,
                                                "",
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                "",
                                                "",
                                                objParametri_Server).Rows(0)("Servizio_Des")

            Dim StatoIniziale_Cod As Integer = 0

            Dim LeggiSequenze As New Agro_Sequenze
            Dim NuovoPassaggioDiStato_cod As Integer = 0
            NuovoPassaggioDiStato_cod = LeggiSequenze.NuovoId_Tabella("PassaggioDiStato_cod",
                                                                      0,
                                                                      _UpperBoundTabelle,
                                                                      objParametri_Server)

            Dim objSequenza As New Agro_Sequenze
            Dim Pratica_Cod = objSequenza.NuovoId_Tabella("Pratiche",
                                                          0,
                                                          _UpperBoundTabelle,
                                                          objParametri_Server)

            Dim Blocco_Data = AGRODATAINIZIO
            Dim Blocco_Username = ""

            If Blocco_Flag = -1 Then
                Blocco_Data = DateTime.Now
                Blocco_Username = objParametri_Server.UtenteUsername
            End If

            InseritaPratica = objPratica_W.Scrivi(Pratica_Cod,
                                                  Servizio_Des,
                                                  piva,
                                                  Cuaa,
                                                  0,
                                                  0,
                                                  0,
                                                  Servizio_Cod,
                                                  Data_Inizio,
                                                  Data_Fine,
                                                  objParametri_Server,
                                                  Date.Now,
                                                  Date.Now,
                                                  objParametri_Server.UsernameOperazione,
                                                  objParametri_Server.UsernameOperazione,
                                                  0,
                                                  Numero,
                                                  Blocco_Flag,
                                                  Blocco_Data,
                                                  Blocco_Username,
                                                  0,
                                                  0)

            If InseritaPratica = True Then

                Pratica_Cod_Inserita = Pratica_Cod

                'Scrivo il solo passaggio di stato a pratica aperta ed indico lo stato attuale di pratica aperta

                StatoIniziale_Cod = objPratiche_R.Leggi_StatoInizialeServizio(Servizio_Cod,
                                                                              "",
                                                                              objParametri_Server)

                ScrivoPassaggioDiStatoIniziale(StatoIniziale_Cod,
                                               NuovoPassaggioDiStato_cod,
                                               Servizio_Cod,
                                               Data_Fine,
                                               Data_Inizio,
                                               Pratica_Cod,
                                               objPraticaStati_W,
                                               objPraticaStati_Attuale_W,
                                               objParametri_Server)

            End If

            Utility.VerificaChiudiTransazione(objParametri_Server, FlagTransazioneLocale)

            ' Sostituzione del trigger tr_up_cascade_update (ramo INSERT): invalido la cache di
            ' visibilita' appoggio degli utenti con filtro pratiche sul servizio della nuova pratica.
            If InseritaPratica AndAlso objParametri_Utenti IsNot Nothing AndAlso Servizio_Cod > 0 Then
                Dim objUpp As New AgronicaCoreUtentiDAL.Utenti_Profili_Pratiche
                objUpp.InvalidaVisibilitaAppoggioXServizio(Servizio_Cod, objParametri_Utenti)
            End If

            r.RispostaConferma = True
            r.RispostaOK = True
            r.RispostaStringa = "Pratica Impostata correttamente"

        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                       Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Utility.VerificaAnnullaTransazione(objParametri_Server, FlagTransazioneLocale)

        Finally

            Utility.VerificaChiudiConnessione(objParametri_Server, FlagConnessioneLocale)

        End Try

        Return r

    End Function

    Private Sub ScrivoPassaggioDiStatoIniziale(ByVal Stato_cod As enum_Servizi_Stati,
                                               ByVal NuovoPassaggioDiStato_cod As Integer,
                                               ByVal Servizio_Cod As Integer,
                                               ByVal DataFinePratica As Date,
                                               ByVal DataApertura As Date,
                                               ByVal Pratica_Cod As Integer,
                                               ByVal objPraticaStati_W As AgronicaCoreProfilazioneDAL.Pratiche_Stati_W,
                                               ByVal objPraticaStati_Attuale_W As AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_W,
                                               ByRef objParametri_Server As AgronicaCoreParametri)

        objPraticaStati_W.Scrivi(Pratica_Cod,
                                 Stato_cod,
                                 Servizio_Cod,
                                 DataApertura,
                                 DataFinePratica,
                                 "",
                                 0,
                                 NuovoPassaggioDiStato_cod,
                                 objParametri_Server,
                                 Now,
                                 Now,
                                 objParametri_Server.UsernameOperazione,
                                 objParametri_Server.UsernameOperazione)

        objPraticaStati_Attuale_W.Scrivi(Pratica_Cod,
                                         Stato_cod,
                                         "",
                                         DataApertura,
                                         DataFinePratica,
                                         objParametri_Server,
                                         Now,
                                         Now,
                                         objParametri_Server.UsernameOperazione,
                                         objParametri_Server.UsernameOperazione)

    End Sub

End Class
