Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieDAL
Imports Quartz

Public Interface IImportatore

    ReadOnly Property JOBKEY As String
    ReadOnly Property TRIGGERKEY As String
    ReadOnly Property GROUPKEY As String

    Property Parametri_Dinamici As Object

    Sub Start()
    Function StartOnDemand(ByVal contesto As enum_Contesto_Integrazione_Macchine_Lavorazione, ByRef schedulatore As IScheduler) As Boolean
    Sub SetEnabled(ByVal enabled As Boolean)

End Interface

Public MustInherit Class ImportatoreBase : Implements IImportatore

    Protected _enabled As Boolean
    Protected _objParametri As ObjParametri
    Protected _pollingInterval As Integer
    Protected _scheduler As IScheduler
    Protected _configurazione As ConfigurazioneImportatore
    Protected _configurazione_Servizio As Configurazione_Servizio
    Protected _parametri_Dinamici As Object

    Protected Const JOB_DATA_CONFIGURAZIONE_SERVIZIO As String = "configurazione_Servizio"
    Protected Const JOB_DATA_OBJPARAMETRI As String = "objParametri"
    Protected Const JOB_DATA_CONFIGURAZIONE_IMPORTATORE As String = "configurazione_Importatore"
    Protected Const JOB_DATA_PARAMETRI_DINAMICI As String = "parametri_Dinamici"
    Protected Const JOB_DATA_TIPO_JOB As String = "tipoJob"
    Protected Const JOB_DATA_CONTESTO As String = "contesto"
    Protected Const JOB_DATA_LOGGER As String = "logger"

    Public MustOverride ReadOnly Property JOBKEY As String Implements IImportatore.JOBKEY
    Public MustOverride ReadOnly Property GROUPKEY As String Implements IImportatore.GROUPKEY
    Public MustOverride ReadOnly Property TRIGGERKEY As String Implements IImportatore.TRIGGERKEY
    Public MustOverride Property Parametri_Dinamici As Object Implements IImportatore.Parametri_Dinamici

    Public Sub New()

    End Sub

    Public Sub New(
                   ByVal configurazione_Servizio As Configurazione_Servizio,
                   ByVal configurazione As ConfigurazioneImportatore,
                   ByVal objParametri As ObjParametri,
                   ByVal scheduler As IScheduler)

        _objParametri = objParametri
        _scheduler = scheduler
        _configurazione = configurazione
        _configurazione_Servizio = configurazione_Servizio

    End Sub
    Public Sub New(
                   ByVal configurazione_Servizio As Configurazione_Servizio,
                   ByVal configurazione As ConfigurazioneImportatore,
                   ByVal objParametri As ObjParametri,
                   ByVal scheduler As IScheduler,
                   ByRef parametriDinamici As Object)
        Me.New(configurazione_Servizio, configurazione, objParametri, scheduler)
        _parametri_Dinamici = parametriDinamici
    End Sub

    Public MustOverride Sub SetEnabled(enabled As Boolean) Implements IImportatore.SetEnabled
    Public MustOverride Sub Start() Implements IImportatore.Start

    Public MustOverride Function StartOnDemand(ByVal contesto As enum_Contesto_Integrazione_Macchine_Lavorazione, ByRef schedulatore As IScheduler) As Boolean Implements IImportatore.StartOnDemand

End Class

Public Enum TipoJob
    NonDefinito = 0
    Schedulato = 1
    OnDemand = 2
End Enum