Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Quartz

Public Class Esportatore_RsService_Ulma : Inherits ImportatoreBase

    Protected Const _JOBKEY As String = "Esportatore_RsService_Ulma_Job"
    Protected Const _TRIGGERKEY As String = "Esportatore_RsService_Ulma_Trigger"
    Protected Const _GROUPKEY As String = "Esportatori"

    Public Overrides ReadOnly Property JOBKEY As String
        Get
            Return _JOBKEY
        End Get
    End Property

    Public Overrides ReadOnly Property GROUPKEY As String
        Get
            Return _GROUPKEY
        End Get
    End Property

    Public Overrides ReadOnly Property TRIGGERKEY As String
        Get
            Return _TRIGGERKEY
        End Get
    End Property

    Public Overrides Property Parametri_Dinamici As Object
        Get
            Return _parametri_Dinamici
        End Get
        Set(value As Object)
            _parametri_Dinamici = value
        End Set
    End Property

    Public Sub New(
                  ByVal configurazione_Servizio As Configurazione_Servizio,
                  ByVal ci As ConfigurazioneImportatore,
                  ByVal objParametri As ObjParametri,
                  ByVal scheduler As IScheduler
                  )
        MyBase.New(configurazione_Servizio, ci, objParametri, scheduler)
    End Sub

    Public Sub New(
                  ByVal configurazione_Servizio As Configurazione_Servizio,
                  ByVal ci As ConfigurazioneImportatore,
                  ByVal objParametri As ObjParametri,
                  ByVal scheduler As IScheduler,
                  ByRef parametriDinamici As Object
                  )
        MyBase.New(configurazione_Servizio, ci, objParametri, scheduler, parametriDinamici)
    End Sub

    Public Overrides Sub SetEnabled(enabled As Boolean)
        Throw New NotImplementedException()
    End Sub

    Public Overrides Function StartOnDemand(ByVal contesto As enum_Contesto_Integrazione_Macchine_Lavorazione, ByRef schedulatore As IScheduler) As Boolean

        Dim nomeJob As String = String.Format("{0}-{1}", _JOBKEY, _configurazione.IdServizio)
        Dim job As IJobDetail = JobBuilder.Create(Of Esportatore_RsService_Ulma_Job).
                                   WithIdentity(nomeJob, _GROUPKEY).
                                   Build

        Dim logger = New Logger(_configurazione_Servizio, _configurazione, _objParametri)

        job.JobDataMap.Add(JOB_DATA_CONFIGURAZIONE_SERVIZIO, _configurazione_Servizio)
        job.JobDataMap.Add(JOB_DATA_OBJPARAMETRI, _objParametri)
        job.JobDataMap.Add(JOB_DATA_CONFIGURAZIONE_IMPORTATORE, _configurazione)
        If Not IsNothing(_parametri_Dinamici) Then
            job.JobDataMap.Add(JOB_DATA_PARAMETRI_DINAMICI, _parametri_Dinamici)
        End If
        job.JobDataMap.Add(JOB_DATA_CONTESTO, contesto)
        job.JobDataMap.Add(JOB_DATA_TIPO_JOB, TipoJob.OnDemand)
        job.JobDataMap.Add(JOB_DATA_LOGGER, logger)

        Dim nomeTrigger As String = String.Format("{0}-{1}", _TRIGGERKEY, _configurazione.IdServizio)
        Dim trigger As ITrigger = TriggerBuilder.Create().WithIdentity(nomeTrigger, _GROUPKEY).
                                    StartNow().
                                    WithSimpleSchedule(Function(x) x.WithIntervalInSeconds(0).WithRepeatCount(0)).
                                    Build

        schedulatore.ScheduleJob(job, trigger)

        logger.InfoFormat("Task '{0}' aggiunto allo Schedulatore", nomeJob)

        Return True

    End Function

    Public Overrides Sub Start()

        Dim nomeJob As String = String.Format("{0}-{1}", _JOBKEY, _configurazione.IdServizio)
        Dim job As IJobDetail = JobBuilder.Create(Of Esportatore_RsService_Ulma_Job).
                                   WithIdentity(nomeJob, _GROUPKEY).
                                   Build
        Dim logger = New Logger(_configurazione_Servizio, _configurazione, _objParametri)

        job.JobDataMap.Add(JOB_DATA_CONFIGURAZIONE_SERVIZIO, _configurazione_Servizio)
        job.JobDataMap.Add(JOB_DATA_OBJPARAMETRI, _objParametri)
        job.JobDataMap.Add(JOB_DATA_CONFIGURAZIONE_IMPORTATORE, _configurazione)
        If Not IsNothing(_parametri_Dinamici) Then
            job.JobDataMap.Add(JOB_DATA_PARAMETRI_DINAMICI, _parametri_Dinamici)
        End If
        job.JobDataMap.Add(JOB_DATA_TIPO_JOB, TipoJob.Schedulato)
        job.JobDataMap.Add(JOB_DATA_LOGGER, logger)

        Dim nomeTrigger As String = String.Format("{0}-{1}", _TRIGGERKEY, _configurazione.IdServizio)
        Dim trigger As ITrigger = TriggerBuilder.Create().WithIdentity(nomeTrigger, _GROUPKEY).
                                    StartNow().
                                    WithSimpleSchedule(Function(x) x.WithIntervalInSeconds(_configurazione.IntervalloPollingInSecondi).
                                    RepeatForever()).
                                    Build

        _scheduler.ScheduleJob(job, trigger)

        logger.InfoFormat("Task '{0}' aggiunto allo Schedulatore", nomeJob)

    End Sub


#Region "Classe di lavoro (JOB)"

    Private Class Esportatore_RsService_Ulma_Job : Implements IJob

        Public Function Execute(context As IJobExecutionContext) As Task Implements IJob.Execute

            Dim risultato As String = String.Empty
            Dim configurazione_servizio As Configurazione_Servizio = Nothing
            Dim objParametri As ObjParametri = Nothing
            Dim configurazioneImportatore As ConfigurazioneImportatore = Nothing
            Dim parametriDinamici As Object = Nothing
            Dim contesto As enum_Contesto_Integrazione_Macchine_Lavorazione
            Dim tipoJob As TipoJob = TipoJob.NonDefinito
            Dim logger As Logger = Nothing

            context.JobDetail.JobDataMap().TryGetValue(JOB_DATA_TIPO_JOB, tipoJob)
            context.JobDetail.JobDataMap().TryGetValue(JOB_DATA_CONFIGURAZIONE_SERVIZIO, configurazione_servizio)
            context.JobDetail.JobDataMap().TryGetValue(JOB_DATA_CONFIGURAZIONE_IMPORTATORE, configurazioneImportatore)
            context.JobDetail.JobDataMap().TryGetValue(JOB_DATA_OBJPARAMETRI, objParametri)
            context.JobDetail.JobDataMap().TryGetValue(JOB_DATA_PARAMETRI_DINAMICI, parametriDinamici)
            context.JobDetail.JobDataMap().TryGetValue(JOB_DATA_CONTESTO, contesto)
            context.JobDetail.JobDataMap().TryGetValue(JOB_DATA_LOGGER, logger)

            Dim nomeJob As String = String.Format("{0}-{1}", _JOBKEY, configurazioneImportatore.IdServizio)
            If tipoJob <> TipoJob.OnDemand Then

                If ServizioSchedulatoreFactory.Instance.Numero_Istanze_Job(nomeJob, _GROUPKEY) > 1 Then
                    logger.InfoFormat("Mancata esecuzione del task '{0}' perchè già in esecuzione", nomeJob)
                    Return Task.FromResult(String.Format("Il Task {0} sta già girando", nomeJob))
                End If
            End If

            logger.InfoFormat("INIZIO Esecuzione del task '{0}'", nomeJob)

            Try

                Dim servizio As New ServizioEsportazione_RsService_Ulma(configurazione_servizio,
                                                                  configurazioneImportatore,
                                                                  objParametri,
                                                                  parametriDinamici)

                Dim rval As RispostaStandard = Nothing

                Select Case contesto
                    Case enum_Contesto_Integrazione_Macchine_Lavorazione.Invio_Primo_Ingresso_Lav
                        rval = servizio.AvviaEsportPrimoIngressoConfezionatrice()
                    Case enum_Contesto_Integrazione_Macchine_Lavorazione.Invio_Ordine_Lav
                        rval = New RispostaStandard With {.RispostaOK = True, .RispostaStringa = ""}
                End Select

                risultato = rval.RispostaStringa

            Catch ex As Exception
                risultato = ex.Message
                logger.ErrorFormat("Errore durante l'esecuzione del Task {0}.{1}{2}", nomeJob, Environment.NewLine, risultato)
            End Try

            logger.InfoFormat("FINE Esecuzione del task '{0}'", nomeJob)
            logger.InfoFormat("Risultato del task {0}: {1}{2}", nomeJob, Environment.NewLine, risultato)

            If tipoJob = TipoJob.OnDemand Then
                '---------------------------------------------------------------------------------------------------------
                '1. Questa istruzione chiude TUTTI gli appender del repository:
                ''''logger.Logger.Logger.Repository.Shutdown()
                '---------------------------------------------------------------------------------------------------------
                '2. Invece occorre cercare l'appender relativo al servizio OnDemand e chiudere solo quello.
                '   Anche in questo modo, viene comunque perso il nome file dell'appender.
                ''''For Each app In logger.Logger.Logger.Repository.GetAppenders()
                ''''    If app.Name = logger.Logger.Logger.Name Then
                ''''        app.Close()
                ''''        Exit For
                ''''    End If
                ''''Next
                '---------------------------------------------------------------------------------------------------------
                '3. Per ora risolto settando in Log4net.config gli appender di esportazione con lockingModel = MinimalLock
                '---------------------------------------------------------------------------------------------------------
            End If

            Return Task.FromResult(risultato)

        End Function

    End Class

#End Region

End Class
