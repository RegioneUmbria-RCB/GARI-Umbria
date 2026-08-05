Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Quartz

Public Class Importatore_Malavasi : Inherits ImportatoreBase

    Protected Const _JOBKEY As String = "Importatore_Malavasi_Job"
    Protected Const _TRIGGERKEY As String = "Importatore_Malavasi_Trigger"
    Protected Const _GROUPKEY As String = "Importatori"


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

    Public Overrides Sub SetEnabled(enabled As Boolean)
        Throw New NotImplementedException()
    End Sub

    Public Overrides Function StartOnDemand(ByVal contesto As enum_Contesto_Integrazione_Macchine_Lavorazione, ByRef schedulatore As IScheduler) As Boolean

        Return True

    End Function

    Public Overrides Sub Start()

        Dim nomeJob As String = String.Format("{0}-{1}", _JOBKEY, _configurazione.IdServizio)
        Dim job As IJobDetail = JobBuilder.Create(Of Importatore_Malavasi_Job).
                                   WithIdentity(nomeJob, _GROUPKEY).
                                   Build

        Dim logger = New Logger(_configurazione_Servizio, _configurazione, _objParametri)

        job.JobDataMap.Add(JOB_DATA_CONFIGURAZIONE_SERVIZIO, _configurazione_Servizio)
        job.JobDataMap.Add(JOB_DATA_OBJPARAMETRI, _objParametri)
        job.JobDataMap.Add(JOB_DATA_CONFIGURAZIONE_IMPORTATORE, _configurazione)
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

    Private Class Importatore_Malavasi_Job : Implements IJob

        Public Function Execute(context As IJobExecutionContext) As Task Implements IJob.Execute

            Dim risultato As String = String.Empty
            Dim configurazione_servizio As Configurazione_Servizio = Nothing
            Dim objParametri As ObjParametri = Nothing
            Dim configurazioneImportatore As ConfigurazioneImportatore = Nothing
            Dim logger As Logger = Nothing

            context.JobDetail.JobDataMap().TryGetValue(JOB_DATA_CONFIGURAZIONE_SERVIZIO, configurazione_servizio)
            context.JobDetail.JobDataMap().TryGetValue(JOB_DATA_CONFIGURAZIONE_IMPORTATORE, configurazioneImportatore)
            context.JobDetail.JobDataMap().TryGetValue(JOB_DATA_OBJPARAMETRI, objParametri)
            context.JobDetail.JobDataMap().TryGetValue(JOB_DATA_LOGGER, logger)

            Dim nomeJob As String = String.Format("{0}-{1}", _JOBKEY, configurazioneImportatore.IdServizio)
            If ServizioSchedulatoreFactory.Instance.Numero_Istanze_Job(nomeJob, _GROUPKEY) > 1 Then
                logger.InfoFormat("Mancata esecuzione del task '{0}' perchè già in esecuzione", nomeJob)
                Return Task.FromResult(String.Format("Il Task {0} sta già girando", nomeJob))
            End If

            logger.InfoFormat("INIZIO Esecuzione del task '{0}'", nomeJob)

            Try
                Dim servizio As New ServizioImportazione_Malavasi(configurazione_servizio, configurazioneImportatore, objParametri)
                Dim rval As RispostaStandard = servizio.AvviaImportCalibratriceMalavasi()
                risultato = rval.RispostaStringa

            Catch ex As Exception
                risultato = ex.Message
                logger.ErrorFormat("Errore durante l'esecuzione del Task {0}.{1}{2}", nomeJob, Environment.NewLine, risultato)
            End Try

            logger.InfoFormat("FINE Esecuzione del task '{0}'", nomeJob)
            logger.InfoFormat("Risultato del task {0}: {1}{2}", nomeJob, Environment.NewLine, risultato)

            Return Task.FromResult(risultato)

        End Function

    End Class

#End Region

End Class

