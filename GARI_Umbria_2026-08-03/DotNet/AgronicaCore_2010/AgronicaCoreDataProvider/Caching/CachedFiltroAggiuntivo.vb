
Imports System.Collections.Specialized
Imports System.Runtime.Caching

Public Class CachedFiltroAggiuntivo
    Implements ICacheableData

    Private ReadOnly _cache As ObjectCache = Nothing
    Private ReadOnly _configurazione As ConfigurazioneEstesaSqlProvider = Nothing
    Private Const _cacheKeyPrefix As String = "CACHED_FILTRO_AGGIUNTIVO"

    Public ReadOnly Property CACHE_KEY_PREFIX As String Implements ICacheableData.CACHE_KEY_PREFIX
        Get
            Return _cacheKeyPrefix
        End Get
    End Property

    Sub New()

        _cache = MemoryCache.Default

    End Sub

    Sub New(configurazione As ConfigurazioneEstesaSqlProvider)

        _configurazione = configurazione

        Dim cacheConfig = New NameValueCollection()
        cacheConfig.Add("pollingInterval", "00:05:00")
        cacheConfig.Add("physicalMemoryLimitPercentage", "0")
        cacheConfig.Add("cacheMemoryLimitMegabytes", "100")

        _cache = New MemoryCache("XFiltroAggiuntivoCache", cacheConfig)

    End Sub

    Private Function Ottieni_Chiave_Cache_Completa(
                ByVal stringConnessione As String,
                ByVal stringaSql As String) As String

        Dim nomeServer As String = If(String.IsNullOrEmpty(stringConnessione), "", DataProviderFactory.Instance.Provider.NomeDataBase_FromStringaConnessione(stringConnessione))

        ' NOME DELLA CACHE - nomeServer - nome classe - nomme metodo - stringa sql 
        Return String.Format("{0}§{1}§{2}§{3}§{4}", _cacheKeyPrefix, nomeServer, "", "", stringaSql)

    End Function
    Public Function Leggi(ByVal filtroAggiuntivo As String,
                          Optional ByVal objParametri As AgronicaCoreParametri = Nothing) As FiltroAggiuntivoCacheEntry

        If Not _configurazione.UtilizzaCache Then
            Return Nothing
        End If

        Dim stringaConnessione As String = If(IsNothing(objParametri), "", objParametri.StringaConnessione)

        Dim chiaveCompleta = Ottieni_Chiave_Cache_Completa(stringaConnessione, filtroAggiuntivo)

        Try
            If _cache.Contains(chiaveCompleta) Then

                Dim valoreNellaCache As FiltroAggiuntivoCacheEntry = _cache.Get(chiaveCompleta)
                If valoreNellaCache IsNot Nothing Then
                    Return valoreNellaCache
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try

    End Function
    Public Sub Scrivi(ByVal filtroAggiuntivo As String,
                      ByVal valore As FiltroAggiuntivoCacheEntry,
                      Optional ByVal objParametri As AgronicaCoreParametri = Nothing)

        If Not _configurazione.UtilizzaCache Then
            Return
        End If

        Dim cachePolicy As New CacheItemPolicy With
        {
            .AbsoluteExpiration = DateTimeOffset.MaxValue,
            .SlidingExpiration = New TimeSpan(0, _configurazione.SlidingExpirationInMinuti, 0),
            .Priority = CacheItemPriority.Default
        }

        If Not valore Is Nothing Then

            Dim stringaConnessione As String = If(IsNothing(objParametri), "", objParametri.StringaConnessione)

            Dim chiaveCompleta As String = Ottieni_Chiave_Cache_Completa(stringaConnessione, filtroAggiuntivo)
            If Not _cache.Contains(chiaveCompleta) Then

                Try
                    _cache.Set(chiaveCompleta, valore, cachePolicy)
                Catch ex As Exception

                End Try
            End If
        End If

    End Sub

#Region "Interfaccia ICacheableData"
    Public Function Dammi_Contenuto_Cache(ByVal tipoCache As String) As List(Of Agronica_Oggetto_Cache) Implements ICacheableData.Dammi_Contenuto_Cache

        If IsNothing(_cache) OrElse Not _cache.Any Then
            Return New List(Of Agronica_Oggetto_Cache)
        End If

        Dim oggettiInCache As New List(Of Agronica_Oggetto_Cache)

        For Each kvp As KeyValuePair(Of String, Object) In _cache
            Dim oCache As Agronica_Oggetto_Cache = CacheBase.Ottieni_Oggetto_Cache_Da_Chiave(kvp.Key)
            If Not IsNothing(kvp.Value) Then
                Dim face As FiltroAggiuntivoCacheEntry = DirectCast(kvp.Value, FiltroAggiuntivoCacheEntry)
                Dim tipo As String = If(face.IsOrderBy, "ORDER BY", "FILTRO AGGIUNTIVO")
                oCache.Valore = String.Format("{0} ({1}) -> {2}", face.FiltroAggiuntivoOriginale, tipo, face.FiltroAggiuntivoParsato)
                oCache.Chiave = kvp.Key
                oggettiInCache.Add(oCache)
            End If

        Next

        Return oggettiInCache

    End Function

    Public Function Rimuovi_Elemento_Dalla_Cache(chiave As String) As Boolean Implements ICacheableData.Rimuovi_Elemento_Dalla_Cache

        Return CacheBase.Rimuovi_Elemento_Dalla_Cache(chiave, _cache)

    End Function

    Public Sub Pulisci_Cache() Implements ICacheableData.Pulisci_Cache

        CacheBase.Pulisci_Cache(_cache)

    End Sub

#End Region

End Class

Public Class FiltroAggiuntivoCacheEntry

    Public FiltroAggiuntivoOriginale As String
    Public FiltroAggiuntivoParsato As String
    Public Parametri As List(Of AgroDBParametro)
    Public CreaParametri As Boolean
    Public IsOrderBy As Boolean

End Class


