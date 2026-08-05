Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreModello
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreContabDAL
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Utility_Integrazione_Macchine : Implements IDisposable

    Private _objParametriSuperServer As AgronicaCoreParametri
    Private _objParametriServer As AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreParametri
    Private _configurazioneServizio As Configurazione_Servizio = Nothing
    Private _servizioLavorazioni As LavorazioniHelper
    Private _pathLog4netConfig As String = String.Empty
    Private Const fileLog4netConfig As String = "Log4net.config"

    ''' <summary>
    ''' Costruttore attraverso oggetto configurazione servizio (chiamata GSB da server)
    ''' </summary>
    ''' <param name="objParametriSuperServer"></param>
    ''' <param name="objParametriServer"></param>
    ''' <param name="objParametriUtenti"></param>
    ''' <param name="configurazioneServizio"></param>
    Public Sub New(ByVal objParametriSuperServer As AgronicaCoreParametri,
                   ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtenti As AgronicaCoreParametri,
                   ByVal configurazioneServizio As Configurazione_Servizio)
        EseguiCostruttoreBase(objParametriSuperServer, objParametriServer, objParametriUtenti)
        _configurazioneServizio = configurazioneServizio

    End Sub

    ''' <summary>
    ''' Costruttore attraverso percorso file configurazione log4net (chiamata OnDemand da client)
    ''' </summary>
    ''' <param name="objParametriSuperServer"></param>
    ''' <param name="objParametriServer"></param>
    ''' <param name="objParametriUtenti"></param>
    ''' <param name="pathLog4netConfig"></param>
    Public Sub New(ByVal objParametriSuperServer As AgronicaCoreParametri,
                   ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtenti As AgronicaCoreParametri,
                   ByVal pathLog4netConfig As String)
        EseguiCostruttoreBase(objParametriSuperServer, objParametriServer, objParametriUtenti)
        _pathLog4netConfig = pathLog4netConfig & "\" & fileLog4netConfig
    End Sub

    Private Sub EseguiCostruttoreBase(ByVal objParametriSuperServer As AgronicaCoreParametri,
                                      ByVal objParametriServer As AgronicaCoreParametri,
                                      ByVal objParametriUtenti As AgronicaCoreParametri)
        _objParametriSuperServer = objParametriSuperServer
        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti
        _servizioLavorazioni = New LavorazioniHelper(_objParametriServer, _objParametriServer)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Public Sub Dispose(eseguiPulizia As Boolean)
        If eseguiPulizia Then
            'Ulteriori azioni pulizia classe
        End If
    End Sub

    ''' <summary>
    ''' Verifica se inviare il primo ingresso per l'interfaccia integrazione macchine
    ''' </summary>
    ''' <param name="idMovDet">Id movimento ingresso inserito</param>
    ''' <param name="agroLav">Oggetto lavorazione; se non passato, serve passare Id agenda per lettura</param>
    ''' <param name="idAgendaLettLav">Id agenda per lettura oggetto lavorazione se non passato</param>
    ''' <param name="piva">Piva</param>
    ''' <param name="numIngressiInseriti">Numero ingressi inseriti in transazione</param>
    Public Sub SeInvioPrimoIngressoLavIntegrMacchine(idMovDet As Integer,
                                                     agroLav As AgronicaCoreModello.Lavorazione,
                                                     idAgendaLettLav As Integer,
                                                     piva As String,
                                                     controllaSePrimoIngresso As Boolean)

        Const NomeRoutine = "AgronicaCoreIntegrazioneMacchine.Utility_Integrazione_Macchine.SeInvioPrimoIngressoLavIntegrMacchine()"
        Dim objLog As New LogProvider

        If IsNothing(agroLav) Then
            agroLav = _servizioLavorazioni.LeggiLavorazione(piva, 0, idAgendaLettLav)
            agroLav.IdAgenda = idAgendaLettLav
        End If

        Dim messaggio = String.Empty

        'Se l'inserimento dello scarico è andato a buon fine controllo l'interfacciamento con le macchine
        If idMovDet <> 0 AndAlso Not String.IsNullOrEmpty(agroLav.CodMacchinaLav) Then

            SeLeggiLineaMacchinaLavorazione(agroLav, piva, _objParametriServer)

            If Not IsNothing(_objParametriSuperServer) Then

                Dim objParametri As New ObjParametri With {.SuperServer = _objParametriSuperServer,
                                                           .Server = _objParametriServer,
                                                           .Utenti = _objParametriUtenti}

                Dim sai As ServizioAttivatoreImportazioni = Nothing
                If IsNothing(_configurazioneServizio) Then
                    sai = New ServizioAttivatoreImportazioni(objParametri, _pathLog4netConfig)
                Else
                    sai = New ServizioAttivatoreImportazioni(_configurazioneServizio, objParametri)
                End If

                If Not String.IsNullOrEmpty(agroLav.Linea_Macchina_Lavorazione.Parametri_Export) Then

                    Dim elencoContesti = agroLav.Linea_Macchina_Lavorazione.Parametri_Export.Split(";")

                    Dim invioPrimaPesata = elencoContesti.FirstOrDefault(Function(c) c.ToLowerInvariant.StartsWith(enum_Contesto_Integrazione_Macchine_Lavorazione.Invio_Primo_Ingresso_Lav.ToString().ToLowerInvariant))

                    If Not String.IsNullOrEmpty(invioPrimaPesata) Then

                        Dim contesto = sai.DammiContesto(invioPrimaPesata)

                        Dim idAgendaInvio As Integer = 0
                        Dim lavCodInvio As String = "0"
                        Dim IdAgendaPrimoIngresso As Integer = agroLav.IdAgenda

                        If Not String.IsNullOrEmpty(contesto.PlaceHolder) Then

                            'Invio lavorazione collegata
                            Try

                                If contesto.PlaceHolder <> PlaceHolderContestoIntegrMacchineLav_InvioLavColleg Then
                                    messaggio = "Placeholder non gestito: {0} (Macchina: {1} - Contesto: {2})"
                                    messaggio = String.Format(messaggio, contesto.PlaceHolder, agroLav.CodMacchinaLav,
                                                          enum_Contesto_Integrazione_Macchine_Lavorazione.Invio_Primo_Ingresso_Lav.ToString())
                                    Throw New Exception(messaggio)
                                End If

                                idAgendaInvio = RicercaLavorazioneOrdineColleg(piva, agroLav.IdAgenda, _objParametriServer, lavCodInvio)

                                AggiornaContestoLavorazioneCollegata(contesto, piva, idAgendaInvio, lavCodInvio, _objParametriServer, sai)

                            Catch ex As Exception
                                objLog.Scrivi_LOG(_objParametriServer, NomeRoutine, ex.Message)
                                idAgendaInvio = 0
                            End Try

                        Else

                            'Invio lavorazione corrente
                            idAgendaInvio = agroLav.IdAgenda
                            lavCodInvio = LAVCOD_TRASFORMAZIONI

                        End If

                        If idAgendaInvio <> 0 Then

                            Dim parametri = New With {.Piva = piva,
                                                      .SaCod = 0,
                                                      .IdAgenda = idAgendaInvio,
                                                      .LavCod = lavCodInvio,
                                                      .IdAgendaPrimoIngresso = IdAgendaPrimoIngresso,
                                                      .controllaSePrimoIngresso = controllaSePrimoIngresso}

                            sai.Inizializza(contesto, parametri)

                        End If

                    End If
                End If
            Else
                messaggio = "Parametri super server non impostati (Agenda: {0})"
                messaggio = String.Format(messaggio, agroLav.IdAgenda)
                objLog.Scrivi_LOG(_objParametriServer, NomeRoutine, messaggio)
            End If

        End If

    End Sub

    Private Sub SeLeggiLineaMacchinaLavorazione(ByRef agroLav As AgronicaCoreModello.Lavorazione,
                                                  piva As String,
                                                  objParametriServer As AgronicaCoreParametri)

        'Se sono arrivato da una pagina dove non avevo a disposizione tutte le info della linea macchina, le leggo ora

        If agroLav.Linea_Macchina_Lavorazione Is Nothing Then

            Dim dalMacchineR As New Linee_Macchine_Lavorazione_R

            Dim dt As DataTable = dalMacchineR.Leggi(piva, agroLav.CodMacchinaLav, String.Empty, String.Empty, String.Empty, objParametriServer)

            If dt IsNot Nothing AndAlso dt.Rows.Count = 1 Then
                agroLav.Linea_Macchina_Lavorazione = New Linee_Macchine_Lavorazione With {
                    .Parametri_Import = If(IsDBNull(dt.Rows(0).Item("Parametri_Import")), Nothing, dt.Rows(0).Item("Parametri_Import")),
                    .Parametri_Export = If(IsDBNull(dt.Rows(0).Item("Parametri_Export")), Nothing, dt.Rows(0).Item("Parametri_Export"))
                    }
            End If

        End If

    End Sub

    Private Function RicercaLavorazioneOrdineColleg(piva As String,
                                                   idAgenda As Integer,
                                                   objParametriServer As AgronicaCoreParametri,
                                                   ByRef lavCodColleg As String) As Integer

        Dim objLavBiz As New AgronicaCoreContabBIZ.FF_LavorazioneBIZ
        Dim agendaColleg As AgronicaCoreContabBIZ.FF_AgendaCollegBIZ = objLavBiz.Ricerca_LavOrd_Colleg(piva, idAgenda, objParametriServer)

        If Not String.IsNullOrEmpty(agendaColleg.MessaggioErrore) Then
            Throw New Exception(agendaColleg.MessaggioErrore)
        End If

        lavCodColleg = agendaColleg.LavCodColleg

        Return agendaColleg.IdAgendaColleg
    End Function

    Private Sub AggiornaContestoLavorazioneCollegata(ByRef contesto As ContestoServizioAttivazioneImpExp,
                                                       piva As String,
                                                       idAgendaColleg As Integer,
                                                       lavCodColleg As String,
                                                       objParametriServer As AgronicaCoreParametri,
                                                       sai As ServizioAttivatoreImportazioni)

        'Leggo codice macchina lavorazione collegata

        Dim objMov As New AgronicaCoreContabDAL.Movimenti_R

        Dim dtMov As DataTable = objMov.Leggi(piva, 0, idAgendaColleg, 0, 0, CAU_LINEA_PRODUZIONE,
                                          enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "",
                                          objParametriServer)

        Dim messaggio = String.Empty

        If dtMov.Rows.Count > 0 Then

            'Leggo i dati della macchina collegata

            Dim objLav As New AgronicaCoreModello.Lavorazione

            objLav.CodMacchinaLav = dtMov.Rows(0).Item("Cod_Macchina_Lav")

            SeLeggiLineaMacchinaLavorazione(objLav, piva, objParametriServer)

            'Determino il servizio relativo alla macchina collegata

            Dim elencoContesti = objLav.Linea_Macchina_Lavorazione.Parametri_Export.Split(";")

            Dim invioPrimaPesataDaLavColleg = elencoContesti.FirstOrDefault(Function(c) c.ToLowerInvariant.StartsWith(enum_Contesto_Integrazione_Macchine_Lavorazione.Invio_Primo_Ingresso_Da_Lav_Colleg.ToString().ToLowerInvariant))

            If Not String.IsNullOrEmpty(invioPrimaPesataDaLavColleg) Then

                Dim contestoLavColleg = sai.DammiContesto(invioPrimaPesataDaLavColleg)
                contesto.IdServizio = contestoLavColleg.IdServizio
                contesto.PlaceHolder = contestoLavColleg.PlaceHolder

            Else
                messaggio = "Contesto macchina lavorazione collegata non trovato (AgendaCollegata: {0} - Macchina: {1} - Contesto: {2})"
                messaggio = String.Format(messaggio, idAgendaColleg, objLav.CodMacchinaLav,
                                      enum_Contesto_Integrazione_Macchine_Lavorazione.Invio_Primo_Ingresso_Da_Lav_Colleg.ToString())
                Throw New Exception(messaggio)
            End If

        Else
            messaggio = "Macchina lavorazione collegata non trovata (AgendaCollegata: {0})"
            messaggio = String.Format(messaggio, idAgendaColleg)
            Throw New Exception(messaggio)
        End If

    End Sub

End Class
