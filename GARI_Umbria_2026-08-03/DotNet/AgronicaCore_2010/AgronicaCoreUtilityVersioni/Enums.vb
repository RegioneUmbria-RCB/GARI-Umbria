Public Class Enumerativi

    Public Enum enum_Tipo_Changelog
        Bug = 1
        Feature = 2
        Security = 3
        Performance = 4
    End Enum

    Public Enum enum_AgronicaSitiRelease
        _NESSUNO = 0
        Agenda = 1
        CoreAPI = 2
        CoreWS = 3
        GiasBase = 4
        Analisi = 5
        Audit = 6
        CheckCoop = 7
        GlobalGap = 8
        SicurezzaLavoro = 9
        LabCQ = 10
        MeteoWS = 11
        PDC = 12
        PianiSemina = 13
        PianoConcimazione = 14
        PUA = 15
        Planning = 16
        Profilazione = 17
        AgroGSB = 18
        AgroGSB_BluArancio = 19
        SincroWEB = 20
        WSImportaGIAS = 21
        WSImportaMagazzino = 22
        Stampe = 23
        UMA = 24
        AWS = 25
        AWSEsterni = 26
        WebApiProfilatore = 27
        ProfitosanAPI = 28
        GiasNG = 29
        DomandaIrrigua = 30
        NetCoreAPI = 31
        Portal = 32
        JobScheduler = 33
        NetCoreDataExchange = 34
        QDCACompliance = 35

        Migra = 100
        Aggancio = 101
        ConfigurazioneSiti = 102
    End Enum

    'Se il sito aggiunto è già scritto in json (e quindi il json non è auto-generato)
    'ricordarsi di aggiungere l'enum a funzione Changelog_Agronica_Obj.IsVersioneConJson

    Public Const PROJECT_AGENDA = "Agronica Agenda"
    Public Const PROJECT_CORE_API = "Agronica Core API"
    Public Const PROJECT_CORE_WS = "Agronica Core WS"
    Public Const PROJECT_GIAS_BASE = "Agronica Gias Base"
    Public Const PROJECT_ANALISI = "Agronica Analisi"
    Public Const PROJECT_AUDIT = "Agronica Audit"
    Public Const PROJECT_CHECK_COOP = "Agronica Check COOP"
    Public Const PROJECT_GLOBAL_GAP = "Agronica Global Gap"
    Public Const PROJECT_SICUREZZA_LAVORO = "Agronica Sicurezza Lavoro"
    Public Const PROJECT_LAB_CQ = "Agronica Lab CQ"
    Public Const PROJECT_METEO_WS = "Agronica Meteo WS"
    Public Const PROJECT_PDC = "Agronica PDC"
    Public Const PROJECT_PIANI_SEMINA = "Agronica Piani Semina"
    Public Const PROJECT_PIANO_CONCIMAZIONE = "Agronica Piano Concimazione"
    Public Const PROJECT_PUA = "Agronica PUA"
    Public Const PROJECT_PLANNING = "Agronica Planning"
    Public Const PROJECT_PROFILAZIONE = "Agronica Profilazione"
    Public Const PROJECT_AGROGSB = "Agronica GSB"
    Public Const PROJECT_AGROGSB_BLUARANCIO = "Agronica GSB BluArancio"
    Public Const PROJECT_SINCRO_WEB = "Agronica Sincro WEB"
    Public Const PROJECT_WS_IMPORTA_GIAS = "Agronica WS Importa GIAS"
    Public Const PROJECT_WS_IMPORTA_MAGAZZINO = "Agronica WS Importa MAGAZZINO"
    Public Const PROJECT_STAMPE = "Agronica Stampe"
    Public Const PROJECT_UMA = "Agronica UMA"
    Public Const PROJECT_WEB_SERVICE = "Agronica Web Service"
    Public Const PROJECT_WEB_SERVICE_ESTERNI = "Agronica Web Service ESTERNI"
    Public Const PROJECT_WEB_API_PROFILATORE = "Agronica Web Api Profilatore"
    Public Const PROJECT_PROFITOSAN_API = "Profitosan API"
    Public Const PROJECT_GIAS_NG = "Gias NG"
    Public Const PROJECT_DOMANDA_IRRIGUA = "Agronica Domanda Irrigua"
    Public Const PROJECT_NET_CORE_API = "Agronica NetCore API"
    Public Const PROJECT_NET_CORE_DATA_EXCHANGE = "Agronica NetCore Data Exchange"
    Public Const PROJECT_PORTAL = "Agronica Portal"
    Public Const PROJECT_JOB_SCHEDULER = "Agronica Job Scheduler"
    Public Const PROJECT_QDCA_COMPLIANCE = "Agronica QDCA Compliance"

    Public Const PROJECT_MIGRA = "Migra"
    Public Const PROJECT_AGGANCIO = "Aggancio"
    Public Const PROJECT_CONFIGURAZIONE_SITI = "Configurazione Siti"

    Public Shared ListPossibleUnreleased As New List(Of String) From {"unreleased", "vnext", "prossima", "wip", "prossimo"}

    '--------------------------------------------
    '             SVILUPPATORI PIGRI
    '--------------------------------------------
    Public Const DEV_Funcy = "Anna Funciello"
    Public Const DEV_Uhalid = "Uhalid Abou El Kheir"
    Public Const DEV_Casa = "Giacomo Casadei"
    Public Const DEV_Drudi = "Riccardo Drudi"
    Public Const DEV_Bandera = "Jordi Bandera"
    Public Const DEV_Divarano = "Lorenzo Di Varano"
    Public Const DEV_LC = "Lorenzo Casanova"
    Public Const DEV_Casmatt = "Mattia Casalboni"
    Public Const DEV_Novaga = "Andrea Novaga"
End Class
