Imports Newtonsoft.Json

Public Class RispostaStandard
    Public Property Sessione As Boolean
    Public Property RispostaOK As Boolean
    Public Property RispostaConferma As Boolean
    Public Property ParametroDue As Boolean
    Public Property ParametroDue_stringa As String

    ' @Paolo: Introdotta per la gestione Webservice del Menu Agenda
    'Indica la tipologia di risposta (può cambiare a seconda della pagina)
    ' es: 1 = script JS
    Public Property Tipo As String

    Public Property Errore As String
    Public Property RispostaStringa As String
    Public Property opzioniWatable As opzioniWatable

    <JsonConverter(GetType(ByteArrayConverter))>
    Public Property RispostaCompressa As Byte()
    Public Property ErroriGias As List(Of ErroreGias)
    Public Property TipoDebug As enum_TipoDebug

    Public Compressa As Boolean
    Public Sub New()
        Sessione = True
        RispostaOK = False
        RispostaConferma = False
        Errore = ""
        RispostaStringa = ""
        ParametroDue_stringa = ""
        Tipo = ""
        ParametroDue = False
        opzioniWatable = New opzioniWatable()
    End Sub
End Class

Public Class RispostaStandard(Of tipo)
    Public Property Sessione As Boolean
    Public Property RispostaOK As Boolean
    Public Property RispostaConferma As Boolean
    Public Property RispostaStringa As tipo
    Public Property ParametroDue As Boolean
    Public Property Errore As String
    Public Property opzioniWatable As opzioniWatable

    <JsonConverter(GetType(ByteArrayConverter))>
    Public RispostaCompressa As Byte()

    Public ErroriGias As List(Of ErroreGias)
    Public TipoDebug As enum_TipoDebug
    Public Compressa As Boolean

    Public Sub New()
        Sessione = True
        RispostaOK = True
        Errore = ""
        opzioniWatable = New opzioniWatable()
    End Sub
End Class

Public Class opzioniWatable
    Public Property PrefissoNomeFileExport As String
    Public Property nomeVarDtInSession As String

    Public Sub New()
        PrefissoNomeFileExport = ""
        nomeVarDtInSession = ""
    End Sub
End Class

Public Class ErroreGias
    Public severity As Integer
    Public messaggio As String
    Public ex As String
    Public tipo As Integer
End Class

Public Enum ErroreGias_Tipo
    'data o in generale dato non corretto
    'permessi non validi, mancanza di permessi
    Generico = 0
    LoginFallito = 1
    ConflittoPermessi = 2
    NonGestito = 999
End Enum

Public Enum ErroreGias_Severity
    'data o in generale dato non corretto
    'permessi non validi, mancanza di permessi
    Bloccante = 0
    Warning = 1
    Info = 2
End Enum

Public Enum enum_TipoDebug

    Off = 0
    Soft = 1
    Verbose = 2

End Enum