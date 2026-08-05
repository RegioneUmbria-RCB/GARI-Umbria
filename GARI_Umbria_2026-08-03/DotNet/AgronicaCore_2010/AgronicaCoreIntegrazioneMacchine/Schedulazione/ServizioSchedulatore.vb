Imports System.Collections.Concurrent
Imports System.Collections.Specialized
Imports Quartz
Imports Quartz.Impl
Imports Quartz.Impl.Matchers

Public Class ServizioSchedulatore

    Private ReadOnly _scheduler As IScheduler = Nothing
    Private ReadOnly _jobsCollection As New ConcurrentDictionary(Of String, JobKey)

    Public ReadOnly Property Scheduler() As IScheduler
        Get
            Return _scheduler
        End Get
    End Property
    Public Sub New()

        Dim config As NameValueCollection = New NameValueCollection()
        config.Add("quartz.scheduler.instanceName", "Schedulatore-Principale")
        config.Add("quartz.scheduler.instanceId", "Schedulatore-Principale")

        _scheduler = New StdSchedulerFactory(config).GetScheduler().Result

    End Sub

    Public Sub Avvia()

        _scheduler.Start()

    End Sub

    Public Sub Arresta()

        _scheduler.Shutdown()

    End Sub

    Public Function Numero_Istanze_Job(ByVal chiaveJob As String, ByVal chiaveGruppo As String) As Integer

        Dim jobKey As New JobKey(chiaveJob, chiaveGruppo)
        Dim conteggio As Integer = 0

        Dim jobs = _scheduler.GetCurrentlyExecutingJobs().Result
        For Each j In jobs
            If j.JobDetail.Key.Name.Equals(jobKey.Name) Then
                conteggio += 1
            End If
        Next

        Return conteggio

    End Function

    Public Function Job_Running(ByVal chiaveJob As String, ByVal chiaveGruppo As String) As Boolean

        Dim jobKey As New JobKey(chiaveJob, chiaveGruppo)

        Dim jobs = _scheduler.GetCurrentlyExecutingJobs().Result
        For Each j In jobs
            If j.JobDetail.Key.Name.Equals(jobKey.Name) Then
                Return True
            End If
        Next

        Return False

    End Function

    Public Function Tenta_Aggiunta_Importatore(ByVal jobkey As JobKey) As Boolean

        For Each kvp As KeyValuePair(Of String, JobKey) In _jobsCollection
            If kvp.Value.Name.Equals(jobkey.Name) AndAlso kvp.Value.Group.Equals(jobkey.Group) Then
                Return False
            End If
        Next

        _jobsCollection.TryAdd(String.Format("{0}§{1}", jobkey.Name, jobkey.Group), jobkey)

        Return True

    End Function

    Public Function Tenta_Rimovizione_Importatore(ByVal jobkey As JobKey) As Boolean

        Dim chiave As String = String.Format("{0}§{1}", jobkey.Name, jobkey.Group)

        If _jobsCollection.ContainsKey(chiave) Then
            _scheduler.DeleteJob(jobkey)
            _jobsCollection.TryRemove(String.Format("{0}§{1}", jobkey.Name, jobkey.Group), jobkey)
            Return True
        Else
            Return False
        End If

    End Function

    Public Function ElencoJob() As List(Of String)

        Dim jobs As New List(Of String)
        Dim gruppi = _scheduler.GetJobGroupNames.Result
        For Each gruppo As String In gruppi
            Dim jkeys As List(Of JobKey) = _scheduler.GetJobKeys(GroupMatcher(Of JobKey).GroupEquals(gruppo)).Result.ToList
            For Each k As JobKey In jkeys
                jobs.Add(k.Name)
            Next
        Next

        Return jobs


    End Function

End Class
