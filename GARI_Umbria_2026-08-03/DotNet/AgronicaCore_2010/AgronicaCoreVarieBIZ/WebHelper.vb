Imports System.Configuration
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Web
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreUtility
Imports InData.RequestHeaders
Imports Newtonsoft.Json

Public Class RispostaStandard

    Private _rispostaStringa As String = String.Empty
    Private _rispostaStringaCustom As String = String.Empty
    Private _Errore As String = String.Empty
    Private _forzaCompressioneFuoriDaWebMethod As Boolean = False
    Public Property RispostaStringaCustom As String
        Get
            Return _rispostaStringaCustom
        End Get
        Set(ByVal value As String)
            _rispostaStringaCustom = value
        End Set
    End Property
    Public Property RispostaStringa As String
        Get
            Return _rispostaStringa
        End Get
        Set(ByVal value As String)

            If Not HttpContext.Current.CompressioneRispostaAbilitata Then
                _rispostaStringa = value
            Else
                If Not ChiamatoDaWebMethod() AndAlso Not _forzaCompressioneFuoriDaWebMethod Then
                    _rispostaStringa = value
                Else
                    RispostaCompressa = AgroZip.CompressioneBase64PerJS(1, value, Text.Encoding.UTF8)
                    Compressa = True
                    _rispostaStringa = String.Empty
                End If
            End If

        End Set

    End Property

    Public Sessione As Boolean
    Public RispostaOK As Boolean
    Public RispostaConferma As Boolean
    Public ParametroDue As Boolean
    Public ParametroDue_stringa As String

    ' @Paolo: Introdotta per la gestione Webservice del Menu Agenda
    'Indica la tipologia di risposta (può cambiare a seconda della pagina)
    ' es: 1 = script JS
    Public Tipo As String

    Public RispostaCompressa As Byte()
    Public opzioniWatable As opzioniWatable

    Public ErroriGias As List(Of ErroreGias)

    Public TipoDebug As enum_TipoDebug

    Public Compressa As Boolean


    Public Property Errore As String
        Get
            Return _Errore
        End Get
        Set(ByVal value As String)
            If Not restituisciEccezione() AndAlso value.Contains("###Errore###") Then
                _Errore = "Error"
            Else
                value = value.Replace("###Errore###", "")
                _Errore = value
            End If
        End Set

    End Property


    Public Sub New()
        Sessione = True
        RispostaOK = False
        RispostaConferma = False
        Errore = ""
        RispostaCompressa = Nothing
        ParametroDue_stringa = ""
        Tipo = ""
        ParametroDue = False
        opzioniWatable = New opzioniWatable()
        ErroriGias = New List(Of ErroreGias)
        Compressa = False
    End Sub

    Public Sub New(ByVal forzaCompressione As Boolean)
        Me.New()
        _forzaCompressioneFuoriDaWebMethod = forzaCompressione
    End Sub

    Private Function ChiamatoDaWebMethod() As Boolean

        Dim risultato As Boolean = False

        Try
            Dim st As New StackTrace
            Dim frames = st.GetFrames()
            Dim metodoChiamante As MethodBase = frames(2).GetMethod()


            If IsNothing(metodoChiamante) Then
                Return risultato
            End If

            If metodoChiamante.IsDefined(GetType(WebMethodAttribute), False) Then
                If frames.Count >= 3 Then
                    Dim precedente As MethodBase = frames(3).GetMethod
                    If Not IsNothing(precedente) Then
                        If precedente.Name.ToLowerInvariant = "InvokeMethod".ToLowerInvariant Then
                            risultato = True
                        Else
                            risultato = False
                        End If
                    End If
                End If
            End If

        Catch ex As Exception

            Return False

        End Try

        Return risultato

    End Function

    Private Shared Function restituisciEccezione() As Boolean
        If ConfigurationManager.AppSettings("RestituisciEccezioniGeneriche") Is Nothing Then
            Return False
        End If
        If CStr(ConfigurationManager.AppSettings("RestituisciEccezioniGeneriche")) = "true" Then
            Return False
        End If
        If CStr(ConfigurationManager.AppSettings("RestituisciEccezioniGeneriche")) = "false" Then
            Return True
        End If

        Return False
    End Function

End Class

Public Class rispostaStandard(Of tipo)

    Private _rispostaStringa As tipo = Nothing
    Private _Errore As String = String.Empty

    Public Property RispostaStringa() As tipo
        Get
            Return _rispostaStringa
        End Get
        Set(ByVal value As tipo)
            _rispostaStringa = value
        End Set
    End Property

    Public Sessione As Boolean
    Public RispostaOK As Boolean
    Public RispostaConferma As Boolean
    Public RispostaCompressa As Byte()
    Public ParametroDue As Boolean
    Public opzioniWatable As opzioniWatable
    Public ErroriGias As List(Of ErroreGias)
    Public TipoDebug As enum_TipoDebug
    Public Compressa As Boolean

    Public Property Errore As String
        Get
            Return _Errore
        End Get
        Set(ByVal value As String)
            If Not restituisciEccezione() AndAlso value.Contains("###Errore###") Then
                _Errore = "Error"
            Else
                value = value.Replace("###Errore###", "")
                _Errore = value
            End If
        End Set

    End Property

    Public Sub New()
        Sessione = True
        RispostaOK = True
        RispostaCompressa = Nothing
        Errore = ""
        opzioniWatable = New opzioniWatable()
        ErroriGias = New List(Of ErroreGias)
        Compressa = False
    End Sub

    Private Shared Function restituisciEccezione() As Boolean
        If ConfigurationManager.AppSettings("RestituisciEccezioniGeneriche") Is Nothing Then
            Return False
        End If
        If CStr(ConfigurationManager.AppSettings("RestituisciEccezioniGeneriche")) = "true" Then
            Return False
        End If
        If CStr(ConfigurationManager.AppSettings("RestituisciEccezioniGeneriche")) = "false" Then
            Return True
        End If

        Return False
    End Function

End Class

Public Class opzioniWatable
    Public PrefissoNomeFileExport As String
    Public nomeVarDtInSession As String

    Public Sub New()
        PrefissoNomeFileExport = ""
        nomeVarDtInSession = ""
    End Sub
End Class

Public Class UpdaterInfo_Response

    Public messaggio As String
    Public errori As String
    Public dati As List(Of UpdaterInfo_Detail)

    Public Sub New()
        messaggio = ""
        errori = ""
        dati = Nothing
    End Sub

End Class

Public Class UpdaterInfo_Detail
    Public Property PIVA As String
    Public Property RagioneSociale As String

    Public Sub New()
        PIVA = ""
        RagioneSociale = ""
    End Sub
End Class

Public Class DatiAttivazioneApp
    Public Property PivaSuperUser As String
    Public Property VersioneAPP As String
    Public Sub New()
        PivaSuperUser = ""
        VersioneAPP = ""
    End Sub
End Class

Public Module WebHelperExtensions

    <Extension()>
    Public Function Ottieni_Client_TimeZone_Info(context As HttpContext) As ClientTimeZoneInfo

        If IsNothing(context.Request) Then
            Return Nothing
        End If
        If IsNothing(context.Request.Headers) Then
            Return Nothing
        End If
        If IsNothing(context.Request.Headers("x-timezone")) Then
            Return Nothing
        End If

        Dim reqHed As String = context.Request.Headers("x-timezone")

        If String.IsNullOrEmpty(reqHed) Then
            Return Nothing
        End If

        Return JsonConvert.DeserializeObject(Of ClientTimeZoneInfo)(reqHed)

    End Function

    <Extension()>
    Public Function CompressioneRispostaAbilitata(context As HttpContext) As Boolean


        ' Controllo se richiesta la compressione attraverso apposito custom header
        If IsNothing(HttpContext.Current) Then
            Return False
        End If
        If IsNothing(HttpContext.Current.Request) Then
            Return False
        End If
        If IsNothing(HttpContext.Current.Request.Headers) Then
            Return False
        End If
        If IsNothing(HttpContext.Current.Request.Headers("x-compressione")) Then
            Return False
        End If

        Dim richiestaCompressione = CBool(HttpContext.Current.Request.Headers("x-compressione"))
        If Not richiestaCompressione Then
            Return False
        End If

        Dim compressioneAbilitata = New ConfigurazioneAjax().AbilitaCompressioneRisposte

        If Not IsNothing(ConfigurazioneAjaxFactory.Instance(Nothing)) Then
            compressioneAbilitata = ConfigurazioneAjaxFactory.Instance(Nothing).AbilitaCompressioneRisposte
        End If

        Return richiestaCompressione AndAlso compressioneAbilitata


    End Function



End Module

