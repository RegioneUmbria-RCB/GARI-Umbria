Imports System.Collections.Specialized
Imports System.IO
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieDAL
Imports log4net
Imports Newtonsoft.Json
Imports Quartz
Imports Quartz.Impl

Public Class ServizioAttivatoreImportazioni

    Private _importatori As Importatori = Nothing
    Private _configurazione_Servizio As Configurazione_Servizio = Nothing
    Private _objParametri As ObjParametri = Nothing

    Private ReadOnly _log As ILog = Nothing

    Public Sub New(ByVal configurazione_Servizio As Configurazione_Servizio,
                   ByVal importatori As Importatori,
                   ByVal objParametri As ObjParametri)
        _importatori = importatori
        _configurazione_Servizio = configurazione_Servizio
        _objParametri = objParametri
        GetLogger(_log)
    End Sub

    Public Sub New(ByVal configurazione_Servizio As Configurazione_Servizio, ByVal objParametri As ObjParametri)
        _configurazione_Servizio = configurazione_Servizio
        _objParametri = objParametri
        GetLogger(_log)
    End Sub

    Public Sub New(ByVal objparametri As ObjParametri)
        _objParametri = objparametri
        _configurazione_Servizio = Leggi_Configurazione_Servizio(
                                        objparametri.SuperServer.PivaSuperUser,
                                        enum_Id_Servizio.GiasOnline,
                                        enum_Tipi_Servizi_Background.Integrazione_Macchine_Lavorazione,
                                        objparametri.SuperServer)
    End Sub

    Public Sub New(ByVal objparametri As ObjParametri, ByVal log4netConfigPath As String)
        Me.New(objparametri)

        If Not String.IsNullOrEmpty(log4netConfigPath) Then

            If File.Exists(log4netConfigPath) Then

                'Imposta proprietà previste dal file di configurazione
                Dim configuraLog4Net As New ConfiguratoreLog4Net
                If IsNothing(_configurazione_Servizio) OrElse String.IsNullOrEmpty(_configurazione_Servizio.Parametri_Extra) Then
                    configuraLog4Net.ImpostaProprietaDefault(log4netConfigPath)
                Else
                    configuraLog4Net.ImpostaProprieta(_configurazione_Servizio.Parametri_Extra)
                End If

                'Apertura file configurazione e inizializzazione log4net
                Using fs As New FileStream(log4netConfigPath, FileMode.Open)
                    log4net.Config.XmlConfigurator.Configure(fs)
                    fs.Close()
                End Using

                GetLogger(_log)

            End If

        End If

    End Sub

    Private Sub GetLogger(ByRef logger As ILog)
        logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
        logger.Info("GetLogger: " & logger.Logger.Name)
        logger.Info("GetCurrentLoggers:")
        Dim numCurLog As Integer = 0
        For Each curLog In logger.Logger.Repository.GetCurrentLoggers()
            numCurLog += 1
            logger.InfoFormat("{0}. {1}", numCurLog, curLog.Name)
        Next
    End Sub

    Public Function AvviaTutto() As Boolean

        ServizioSchedulatoreFactory.Instance.Avvia()
        Return True

    End Function

    Public Function DammiContesto(ByVal stringaAttivatore As String) As ContestoServizioAttivazioneImpExp

        Dim params = stringaAttivatore.Split("=")

        Dim contesto As String = String.Empty
        Dim idServizio As String = String.Empty
        Dim placeHolder As String = String.Empty
        Const placeHolderChar As String = "#"

        If params.Count >= 1 Then
            contesto = params(0)
        End If

        If params.Count >= 2 Then
            Dim valoreContesto As String = params(1)
            If valoreContesto.StartsWith(placeHolderChar) AndAlso valoreContesto.EndsWith(placeHolderChar) Then
                placeHolder = valoreContesto
            Else
                idServizio = valoreContesto
            End If
        End If

        Return New ContestoServizioAttivazioneImpExp With
        {
            .Evento = DirectCast([Enum].Parse(GetType(enum_Contesto_Integrazione_Macchine_Lavorazione), contesto), enum_Contesto_Integrazione_Macchine_Lavorazione),
            .Tipo = "",
            .IdServizio = idServizio,
            .PlaceHolder = placeHolder
        }

    End Function

    Public Function Inizializza(ByVal contesto As ContestoServizioAttivazioneImpExp,
                                Optional parametriDinamici As Object = Nothing) As String

        Try

            If IsNothing(_importatori) Then
                _importatori = JsonConvert.DeserializeObject(Of Importatori)(_configurazione_Servizio.Parametri_Extra)
            End If

            'Verifica Univocità degli id servizio
            If _importatori.Configurazioni.Count <> _importatori.Configurazioni.Distinct(New ConfiguratoreImportatoreComparer()).Count Then
                Throw New Exception("Trovati più servizi con lo stesso IdServizio")
            End If

            Dim impExp As ConfigurazioneImportatore = _importatori.Configurazioni.FirstOrDefault(Function(i) i.IdServizio.ToLower.Equals(contesto.IdServizio.ToLower))
            If IsNothing(impExp) Then
                Return String.Format("Servizio non trovato in configurazione: {0} ", contesto.IdServizio)
            End If

            Dim config As NameValueCollection = New NameValueCollection()
            config.Add("quartz.scheduler.instanceName", "Schedulatore-OnDemand")
            config.Add("quartz.scheduler.instanceId", "Schedulatore-OnDemand")

            Dim factory = New StdSchedulerFactory(config)
            Dim schedulatore As IScheduler = factory.GetScheduler().Result
            schedulatore.Start()
            Dim tasks As List(Of Task) = New List(Of Task)

            Dim lavoro As Task(Of Boolean) = Nothing

            Dim ps As ObjParametri = DammiNuovoObjParametri()
            Dim args = New List(Of Object) From {_configurazione_Servizio, impExp, ps, schedulatore}
            If Not IsNothing(parametriDinamici) Then
                args.Add(parametriDinamici)
            End If

            Dim importatore As IImportatore = IstanziaImportatore(impExp.Type, args.ToArray)
            Dim jk As JobKey = Nothing
            If Not IsNothing(importatore) Then
                jk = New JobKey(importatore.JOBKEY, importatore.GROUPKEY)
                If impExp.Abilitato AndAlso impExp.OnDemand Then
                    ' importatore abilitato (lo inizializzo se non è già stato inizializzato
                    lavoro = Task.Factory.StartNew(Function() importatore.StartOnDemand(contesto.Evento, schedulatore))
                    tasks.Add(lavoro)
                End If
            End If

            If Not IsNothing(lavoro) Then
                Task.WaitAll(tasks.ToArray)
                Dim res = lavoro.Result
            End If

            Task.Delay(TimeSpan.FromSeconds(1)).GetAwaiter().GetResult()
            schedulatore.Shutdown().GetAwaiter().GetResult()

            Return String.Empty

        Catch ex As Exception
            _log.Error(ex.Message)
            Return ex.Message
        Finally
            '_log.Logger.Repository.Shutdown()
        End Try

    End Function

    Public Function Inizializza() As String

        Try

            If IsNothing(_importatori) Then
                _importatori = JsonConvert.DeserializeObject(Of Importatori)(_configurazione_Servizio.Parametri_Extra)
            End If

            'Verifica Univocità degli id servizio
            If _importatori.Configurazioni.Count <> _importatori.Configurazioni.Distinct(New ConfiguratoreImportatoreComparer()).Count Then
                Throw New Exception("Trovati più servizi con lo stesso IdServizio")
            End If

            Dim schedulatore As IScheduler = ServizioSchedulatoreFactory.Instance.Scheduler
            Dim tasks As List(Of Task) = New List(Of Task)

            For Each c As ConfigurazioneImportatore In _importatori.Configurazioni

                Dim ps As ObjParametri = DammiNuovoObjParametri()
                Dim args = New List(Of Object) From {_configurazione_Servizio, c, ps, schedulatore}

                Dim importatore As IImportatore = IstanziaImportatore(c.Type, args.ToArray)
                If Not IsNothing(importatore) Then

                    Dim chiave As String = String.Format("{0}-{1}", importatore.JOBKEY, c.IdServizio)
                    Dim jk As JobKey = New JobKey(chiave, importatore.GROUPKEY)

                    If c.Abilitato Then

                        ' importatore abilitato (lo inizializzo se non è già stato inizializzato
                        If ServizioSchedulatoreFactory.Instance.Tenta_Aggiunta_Importatore(jk) Then
                            If c.OnDemand Then
                                ''tasks.Add(Task.Factory.StartNew(Sub() importatore.StartOnDemand(ServizioSchedulatoreFactory.Instance.Scheduler)))
                            Else
                                tasks.Add(Task.Factory.StartNew(Sub() importatore.Start()))
                            End If

                        End If

                    Else

                        ' disabilito la schedulazione
                        ServizioSchedulatoreFactory.Instance.Tenta_Rimovizione_Importatore(jk)

                    End If


                End If

            Next

            Task.WaitAll(tasks.ToArray)

            Return String.Empty

        Catch ex As Exception
            _log.Error(ex.Message)
            Return ex.Message
        End Try

    End Function

    Private Function DammiNuovoObjParametri() As ObjParametri

        Return New ObjParametri With
                {
                    .Server = New AgronicaCoreParametri With {.StringaConnessione = _objParametri.Server.StringaConnessione,
                                                              .PivaSuperUser = _objParametri.Server.PivaSuperUser,
                                                              .UsernameOperazione = _objParametri.Server.UsernameOperazione},
                    .Utenti = New AgronicaCoreParametri With {.StringaConnessione = _objParametri.Utenti.StringaConnessione,
                                                              .PivaSuperUser = _objParametri.Utenti.PivaSuperUser,
                                                              .UsernameOperazione = _objParametri.Utenti.UsernameOperazione},
                    .SuperServer = New AgronicaCoreParametri With {.StringaConnessione = _objParametri.SuperServer.StringaConnessione,
                                                                   .PivaSuperUser = _objParametri.SuperServer.PivaSuperUser,
                                                                   .UsernameOperazione = _objParametri.SuperServer.UsernameOperazione}
                }

    End Function

    Private Function IstanziaImportatore(ByVal tipo As String, ByVal argomenti As Object()) As IImportatore

        Dim importatore As IImportatore = Nothing

        Try
            importatore = Activator.CreateInstance(Type.GetType(tipo), argomenti)
        Catch ex As Exception
            ' todo log
        End Try

        Return importatore

    End Function

    Private Function Leggi_Configurazione_Servizio(ByVal pivaSuperuser As String,
                                          ByVal idServizio As enum_Id_Servizio,
                                          ByVal tipoSincro As enum_Tipi_Servizi_Background,
                                          ByVal objParametriSuperServer As AgronicaCoreParametri
                                          ) As Configurazione_Servizio

        Dim cs As Configurazione_Servizio = Nothing

        Dim Configurazione_Servizi_R = New AgronicaCoreVarieDAL.Configurazione_Servizi_R()
        cs = Configurazione_Servizi_R.LeggiSingolo(pivaSuperuser, idServizio, tipoSincro, 0, "", objParametriSuperServer)

        Return cs

    End Function

End Class

Public Class ContestoServizioAttivazioneImpExp

    Public Evento As enum_Contesto_Integrazione_Macchine_Lavorazione
    Public Tipo As String
    Public IdServizio As String
    Public PlaceHolder As String

End Class

