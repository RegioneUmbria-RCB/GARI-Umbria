
Imports System.Collections.Concurrent
Imports System.Collections.Specialized
Imports System.Data.Common
Imports System.Reflection
Imports System.Runtime.Caching
Imports Newtonsoft.Json

Public Class CachedDataProvider
    Inherits AgronicaCoreDataProvider.CacheBase
    Implements IReadDataProvider, ICacheableData, IWriteDataProvider

    Private ReadOnly _cache As ObjectCache = Nothing
    Private ReadOnly _cachePolicy As CacheItemPolicy = Nothing

    Private Const _cacheKeyPrefix As String = "CACHED_DATA_PROVIDER"
    Private Const GRUPPO_CACHE_NON_DEFINITO As String = "GRUPPO_CACHE_NON_DEFINITO"

    Public ReadOnly Property CACHE_KEY_PREFIX As String Implements ICacheableData.CACHE_KEY_PREFIX
        Get
            Return _cacheKeyPrefix
        End Get
    End Property

    Public Sub New()

        _cache = MemoryCacheFactory.Instance.CACHE
        _cachePolicy = MemoryCacheFactory.Instance.CachePolicy

    End Sub

#Region "Interfaccia IReadDataProvider"
    Public Overloads Function EseguiQuery_Lettura(
                                        ByRef objConnessione As DbConnection,
                                        ByVal StringaConnessione As String,
                                        ByVal StringaSQL As String,
                                        ByVal DirectoryLOG As String,
                                        ByVal FileLOG As String,
                                        ByVal IdentificatoreUtente As String,
                                        ByVal NomeRoutine As String,
                                        Optional ByVal objTransazione As DbTransaction = Nothing
                                        ) As DataTable Implements IReadDataProvider.EseguiQuery_Lettura

        Dim attributiCache As CacheableAttribute = Nothing
        If Not IsCacheable(attributiCache) Then
            Return MyBase.EseguiQuery_Lettura(objConnessione,
                                                StringaConnessione,
                                                StringaSQL,
                                                DirectoryLOG,
                                                FileLOG,
                                                IdentificatoreUtente,
                                                NomeRoutine,
                                                objTransazione)
        End If

        Dim cacheKey As String = Ottieni_Chiave_Cahche_Completa(attributiCache, StringaConnessione, StringaSQL)

        Dim valoreInCache As DataTable = Me.Leggi(Of DataTable)(cacheKey)
        If IsNothing(valoreInCache) Then
            Dim valoreDaCacheare = MyBase.EseguiQuery_Lettura(objConnessione,
                                                              StringaConnessione,
                                                              StringaSQL,
                                                              DirectoryLOG,
                                                              FileLOG,
                                                              IdentificatoreUtente,
                                                              NomeRoutine,
                                                              objTransazione)

            If valoreDaCacheare IsNot Nothing Then
                Me.Scrivi(Of DataTable)(cacheKey, StringaConnessione, valoreDaCacheare)
            End If

            Return valoreDaCacheare

        Else
            Return valoreInCache
        End If

    End Function

    Public Overloads Function EseguiQuery_Lettura(
                                        ByRef objConnessione As DbConnection,
                                        ByRef objTransazione As DbTransaction,
                                        ByVal StringaConnessione As String,
                                        ByVal StringaSQL As String,
                                        ByVal DirectoryLOG As String,
                                        ByVal FileLOG As String,
                                        ByVal IdentificatoreUtente As String,
                                        ByVal NomeRoutine As String
                                        ) As DataTable Implements IReadDataProvider.EseguiQuery_Lettura

        Return MyBase.EseguiQuery_Lettura(objConnessione,
                                          objTransazione,
                                          StringaConnessione,
                                          StringaSQL,
                                          DirectoryLOG,
                                          FileLOG,
                                          IdentificatoreUtente,
                                          NomeRoutine)
    End Function

    Public Overloads Function EseguiQuery_Lettura_XML(
                                        ByRef objParametri As AgronicaCoreParametri,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String
                                        ) As String Implements IReadDataProvider.EseguiQuery_Lettura_XML

        Return MyBase.EseguiQuery_Lettura_XML(objParametri, StringaSQL, NomeRoutine)

    End Function

    Public Overloads Function EseguiQuery_Lettura(
                                       ByRef objParametri As AgronicaCoreParametri,
                                       ByVal StringaSQL As String,
                                       ByVal NomeRoutine As String,
                                        Optional chiamaScriviLog As Boolean = True
                                       ) As DataTable Implements IReadDataProvider.EseguiQuery_Lettura

        Dim attributiCache As CacheableAttribute = Nothing
        If Not IsCacheable(attributiCache) Then
            Return MyBase.EseguiQuery_Lettura(objParametri, StringaSQL, NomeRoutine)
        End If

        Dim cacheKey As String = Ottieni_Chiave_Cahche_Completa(attributiCache, objParametri.StringaConnessione, StringaSQL.ToOrigin(objParametri))


        Dim valoreInCache As DataTable = Me.Leggi(Of DataTable)(cacheKey)
        If IsNothing(valoreInCache) Then
            Dim valoreDaCacheare = MyBase.EseguiQuery_Lettura(objParametri,
                                                              StringaSQL,
                                                              NomeRoutine,
                                                              chiamaScriviLog:=chiamaScriviLog
                                                              )

            If valoreDaCacheare IsNot Nothing Then
                Me.Scrivi(Of DataTable)(cacheKey, objParametri.StringaConnessione, valoreDaCacheare)
            End If

            Return valoreDaCacheare

        Else
            Return valoreInCache
        End If

    End Function

    Public Overloads Function EseguiQuery_Lettura(
                                       ByRef objParametri As AgronicaCoreParametri,
                                       ByVal StringaSQL As String,
                                       ByVal NomeRoutine As String,
                                       ByRef DataSet2Fill As DataSet,
                                       ByVal strNomeDtNelDS As String
                                       ) As Boolean Implements IReadDataProvider.EseguiQuery_Lettura


        Return MyBase.EseguiQuery_Lettura(objParametri,
                                          StringaSQL,
                                          NomeRoutine,
                                          DataSet2Fill,
                                          strNomeDtNelDS)

    End Function

    <Obsolete("1.Da utilizzare quella con objParametri")>
    Public Overloads Function EseguiQuery_Lettura(
                                       ByRef StringaConnessione As String,
                                       ByVal StringaSQL As String,
                                       ByVal NomeRoutine As String
                                       ) As DataTable Implements IReadDataProvider.EseguiQuery_Lettura

        Dim attributiCache As CacheableAttribute = Nothing
        If Not IsCacheable(attributiCache) Then
            Return MyBase.EseguiQuery_Lettura(StringaConnessione, StringaSQL, NomeRoutine)
        End If

        Dim cacheKey As String = Ottieni_Chiave_Cahche_Completa(attributiCache, StringaConnessione, StringaSQL.ToOrigin)

        Dim valoreInCache As DataTable = Me.Leggi(Of DataTable)(cacheKey)
        If IsNothing(valoreInCache) Then
            Dim valoreDaCacheare = MyBase.EseguiQuery_Lettura(StringaConnessione,
                                                              StringaSQL,
                                                              NomeRoutine)

            If valoreDaCacheare IsNot Nothing Then
                Me.Scrivi(Of DataTable)(cacheKey, StringaConnessione, valoreDaCacheare)
            End If

            Return valoreDaCacheare

        Else
            Return valoreInCache
        End If


    End Function

    Public Overloads Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri,
                                        StringaSQL As String,
                                        parametri As Dictionary(Of String, Object),
                                        NomeRoutine As String) As DataTable Implements IReadDataProvider.EseguiQuery_Lettura

        Return MyBase.EseguiQuery_Lettura(objParametri, StringaSQL, parametri, NomeRoutine)

    End Function

    Public Overloads Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, ByVal parametri As List(Of DbParameter), NomeRoutine As String) As DataTable Implements IReadDataProvider.EseguiQuery_Lettura

        Return Nothing

    End Function

    Private Overloads Function EseguiQuery_Lettura_jSon(ByRef objParametri As AgronicaCoreParametri,
                                                        StringaSQL As String,
                                                        NomeRoutine As String) As String Implements IReadDataProvider.EseguiQuery_Lettura_jSon

        Return MyBase.EseguiQuery_Lettura_jSon(objParametri, StringaSQL, NomeRoutine)

    End Function

#End Region

#Region "Interfaccia ICacheableData"

    Public Function Dammi_Contenuto_Cache(ByVal tipoCache As String) As List(Of Agronica_Oggetto_Cache) Implements ICacheableData.Dammi_Contenuto_Cache
        If IsNothing(_cache) OrElse Not _cache.Any Then
            Return New List(Of Agronica_Oggetto_Cache)
        End If
        Dim oggettiInCache As New List(Of Agronica_Oggetto_Cache)

        Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
        For Each kvp As KeyValuePair(Of String, Object) In _cache
            If Not IsNothing(kvp.Value) Then

                Dim oc = DirectCast(kvp.Value, Agronica_Oggetto_Cache)
                If Not IsNothing(oc.Valore) Then
                    Dim dt As DataTable = DirectCast(oc.Valore, DataTable)
                    oc.ValoreJson = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
                    oc.NumeroRisultati = dt.Rows.Count
                End If
                ''oc.Valore = String.Empty
                oc.Chiave = kvp.Key
                oggettiInCache.Add(oc)
            End If
        Next

        Return oggettiInCache
    End Function

    Public Overloads Function Rimuovi_Elemento_Dalla_Cache(chiave As String) As Boolean Implements ICacheableData.Rimuovi_Elemento_Dalla_Cache

        Return CacheBase.Rimuovi_Elemento_Dalla_Cache(chiave, _cache)

    End Function

    Private Overloads Sub Pulisci_Cache() Implements ICacheableData.Pulisci_Cache

        CacheBase.Pulisci_Cache(_cache)

    End Sub
#End Region

#Region "Interfaccia IWriteDataProvider"

    Public Overloads Function EseguiQuery_InsertParametrizzata(ByRef objParametri As AgronicaCoreParametri,
                                                               Nome_Tabella As String,
                                                               strCampi As String,
                                                               strValori As String) As Boolean Implements IWriteDataProvider.EseguiQuery_InsertParametrizzata

        Return MyBase.EseguiQuery_InsertParametrizzata(objParametri, Nome_Tabella, strCampi, strValori)

    End Function

    Public Overloads Function EseguiQuery_Scrittura(ByRef objParametri As AgronicaCoreParametri,
                                                    StringaSQL As String,
                                                    NomeRoutine As String) As Boolean Implements IWriteDataProvider.EseguiQuery_Scrittura

        Dim risultato As Boolean = MyBase.EseguiQuery_Scrittura(objParametri, StringaSQL, NomeRoutine)
        Try
            Dim attributiCache As ClearCacheAttribute = Nothing
            If IsClearableCache(attributiCache) Then
                Dim nomeServer As String = DataProviderFactory.Instance.Provider.NomeDataBase_FromStringaConnessione(objParametri.StringaConnessione)
                Dim nomeGruppoCache = Ottieni_Nome_Gruppo_Cache_Lettura_Collegato()
                Pulisci_Cache_Puntuale(_cacheKeyPrefix, nomeServer, nomeGruppoCache, _cache)
            End If
        Catch ex As Exception
            Scrivi_LOG(objParametri, "CachedDataProvider.EseguiQuery_Scrittura", ex.Message)
        End Try

        Return risultato

    End Function

    Public Overloads Function EseguiQuery_Scrittura(ByRef objConnessione As DbConnection,
                                                    ByRef objTransazione As DbTransaction,
                                                    StringaConnessione As String,
                                                    StringaSQL As String,
                                                    DirectoryLOG As String,
                                                    FileLOG As String,
                                                    IdentificatoreUtente As String,
                                                    NomeRoutine As String) As Boolean Implements IWriteDataProvider.EseguiQuery_Scrittura

        Return MyBase.EseguiQuery_Scrittura(objConnessione,
                                            objTransazione,
                                            StringaConnessione,
                                            StringaSQL,
                                            DirectoryLOG,
                                            FileLOG,
                                            IdentificatoreUtente,
                                            NomeRoutine)

    End Function

    Public Overloads Function EseguiQuery_Scrittura(ByRef stringaConnessione As String,
                                                    StringaSQL As String,
                                                    NomeRoutine As String) As Boolean Implements IWriteDataProvider.EseguiQuery_Scrittura

        Return MyBase.EseguiQuery_Scrittura(stringaConnessione, StringaSQL, NomeRoutine)

    End Function

    Public Overloads Function EseguiQuery_Scrittura_ParamVarBinary(ByRef objParametri As AgronicaCoreParametri,
                                                                   StringaSQL As String,
                                                                   NomeRoutine As String,
                                                                   CmdParameters As Dictionary(Of String, Byte())) As Boolean Implements IWriteDataProvider.EseguiQuery_Scrittura_ParamVarBinary

        Return MyBase.EseguiQuery_Scrittura_ParamVarBinary(objParametri, StringaSQL, NomeRoutine, CmdParameters)

    End Function

    Public Overloads Function EseguiQuery_ScritturaNum(ByRef objParametri As AgronicaCoreParametri,
                                                       StringaSQL As String,
                                                       NomeRoutine As String,
                                                       ByRef NumeroRecordInteressati As Integer) As Boolean Implements IWriteDataProvider.EseguiQuery_ScritturaNum

        Return MyBase.EseguiQuery_ScritturaNum(objParametri, StringaSQL, NomeRoutine, NumeroRecordInteressati)

    End Function

    Public Overloads Function EseguiQuery_Scrittura_Param(ByRef objParametri As AgronicaCoreParametri,
                                                          StringaSQL As String,
                                                          NomeRoutine As String,
                                                          parametriCommand As List(Of DbParameter)) As Boolean Implements IWriteDataProvider.EseguiQuery_Scrittura_Param

        Return MyBase.EseguiQuery_Scrittura_Param(objParametri, StringaSQL, NomeRoutine, parametriCommand)

    End Function

    Public Overloads Function EseguiQuery_Scrittura_Param(ByRef objParametri As AgronicaCoreParametri,
                                                          ByRef objConnessione As DbConnection,
                                                          ByRef objTransazione As DbTransaction,
                                                          StringaConnessione As String,
                                                          StringaSQL As String,
                                                          NomeRoutine As String,
                                                          parametriCommand As List(Of DbParameter)) As Boolean Implements IWriteDataProvider.EseguiQuery_Scrittura_Param


        Return MyBase.EseguiQuery_Scrittura_Param(objParametri,
                                                  objConnessione,
                                                  objTransazione,
                                                  StringaConnessione,
                                                  StringaSQL,
                                                  NomeRoutine,
                                                  parametriCommand)

    End Function

#End Region

    Private Function Ottieni_Chiave_Cahche_Completa(
                ByVal attributi As CacheableAttribute,
                ByVal stringConnessione As String,
                ByVal stringaSql As String) As String

        Dim nomeIstanza As String = DataProviderFactory.Instance.Provider.NomeIstanza_FromStringaConnessione(stringConnessione)
        Dim nomeServer As String = DataProviderFactory.Instance.Provider.NomeDataBase_FromStringaConnessione(stringConnessione)
        Dim nomeDBCompleto As String = String.Format("{0}§{1}", nomeIstanza, nomeServer)
        Dim nomeGruppoCache = Ottieni_Nome_Gruppo_Cache()

        If String.IsNullOrEmpty(nomeGruppoCache) Then
            nomeGruppoCache = attributi.NomeClasse
        End If

        ' NOME DELLA CACHE - nomeServer - nome classe - nomme metodo - stringa sql 
        Return String.Format("{0}§{1}§{2}§{3}§{4}", _cacheKeyPrefix, nomeDBCompleto, nomeGruppoCache, attributi.NomeMetodo, stringaSql)

    End Function

    Private Function Ottieni_Nome_Gruppo_Cache() As String

        Dim tipo As Type = Me.GetType()

        If tipo.IsDefined(GetType(CachedDataProviderAttribute), False) Then
            Dim attr = DirectCast(tipo.GetCustomAttributes(GetType(CachedDataProviderAttribute), False).FirstOrDefault, CachedDataProviderAttribute)
            Return attr.NomeGruppoCache
        End If

        Return GRUPPO_CACHE_NON_DEFINITO

    End Function

    Private Function Ottieni_Nome_Gruppo_Cache_Lettura_Collegato() As String

        Dim tipo As Type = Me.GetType()

        If tipo.IsDefined(GetType(CachedDataProviderAttribute), False) Then
            Dim attr = DirectCast(tipo.GetCustomAttributes(GetType(CachedDataProviderAttribute), False).FirstOrDefault, CachedDataProviderAttribute)
            If IsNothing(attr.NomeGruppoCacheLetturaCollegato) OrElse String.IsNullOrEmpty(attr.NomeGruppoCacheLetturaCollegato) Then
                Return GRUPPO_CACHE_NON_DEFINITO
            Else
                Return attr.NomeGruppoCacheLetturaCollegato
            End If
        End If

        Return GRUPPO_CACHE_NON_DEFINITO

    End Function

    Private Function Leggi(Of T)(ByVal chiave As String) As T

        Dim valoreNellaCache As Object = _cache.Get(chiave)
        Dim agronicaOggettoCache As Agronica_Oggetto_Cache = Nothing
        Dim valoreRitorno As T = Nothing

        If valoreNellaCache IsNot Nothing Then
            agronicaOggettoCache = DirectCast(valoreNellaCache, Agronica_Oggetto_Cache)
            valoreRitorno = DirectCast(agronicaOggettoCache.Valore, T)
            Return valoreRitorno
        End If

        Return Nothing

    End Function

    Private Sub Scrivi(Of T)(ByVal chiave As String, ByVal stringaConnessione As String, ByVal valore As T)

        Dim oggettoDaCacheare = Ottieni_Oggetto_Cache_Da_Chiave(chiave)
        If IsNothing(oggettoDaCacheare) Then
            Return
        End If

        oggettoDaCacheare.Valore = valore
        _cache.Set(chiave, oggettoDaCacheare, _cachePolicy)

    End Sub

    Private Function IsCacheable(ByRef attributiCache As CacheableAttribute) As Boolean

        attributiCache = Nothing

        If Not MemoryCacheFactory.Instance.EnebaleCacheDataProvider Then
            Return False
        End If

        Dim st As New StackTrace
        Dim mi As MethodBase = st.GetFrame(2).GetMethod()
        If mi.IsDefined(GetType(CacheableAttribute), False) Then
            Dim attr = DirectCast(mi.GetCustomAttributes(GetType(CacheableAttribute), False).FirstOrDefault, CacheableAttribute)
            If attr.Cacheable Then
                If String.IsNullOrEmpty(attr.NomeMetodo) Then
                    attr.NomeMetodo = mi.Name
                End If
                If String.IsNullOrEmpty(attr.NomeClasse) Then
                    attr.NomeClasse = mi.DeclaringType.FullName
                End If
                attributiCache = attr
            End If
            Return attr.Cacheable
        End If

        Return False

    End Function

    Private Function IsClearableCache(ByRef attributi As ClearCacheAttribute) As Boolean

        attributi = Nothing

        If Not MemoryCacheFactory.Instance.EnebaleCacheDataProvider Then
            Return False
        End If

        Dim st As New StackTrace
        Dim mi As MethodBase = st.GetFrame(2).GetMethod()
        If mi.IsDefined(GetType(ClearCacheAttribute), False) Then
            Dim attr = DirectCast(mi.GetCustomAttributes(GetType(ClearCacheAttribute), False).FirstOrDefault, ClearCacheAttribute)
            attributi = attr
            Return True
        End If

        Return False

    End Function

End Class

Public Class MemoryCacheFactory

    Private Shared objSingleton As MemoryCacheFactory
    Private Shared classLocker As New Object()
    Private ReadOnly _cacheCollection As New ConcurrentDictionary(Of String, ICacheableData)
    Private _enebaleCacheDataProvider As Boolean = True

    Private ReadOnly _cache As ObjectCache = Nothing
    Private ReadOnly _widgetCache As ObjectCache = Nothing
    Private ReadOnly _coreWSCache As ObjectCache = Nothing
    Private ReadOnly _cachePolicy As CacheItemPolicy = New CacheItemPolicy With
        {
            .AbsoluteExpiration = DateTimeOffset.MaxValue,
            .SlidingExpiration = New TimeSpan(0, 60, 0),
            .Priority = CacheItemPriority.Default
        }

    Public ReadOnly Property CachePolicy() As CacheItemPolicy
        Get
            Return _cachePolicy
        End Get
    End Property

    Public ReadOnly Property CACHE As ObjectCache
        Get
            Return _cache
        End Get
    End Property

    Public ReadOnly Property WIDGET_CACHE As ObjectCache
        Get
            Return _widgetCache
        End Get
    End Property

    Public ReadOnly Property CACHE_COLLECTION As ConcurrentDictionary(Of String, ICacheableData)
        Get
            Return _cacheCollection
        End Get
    End Property

    Public Property EnebaleCacheDataProvider As Boolean
        Get
            Return _enebaleCacheDataProvider
        End Get
        Set(value As Boolean)
            _enebaleCacheDataProvider = value
        End Set
    End Property

    Public ReadOnly Property COREWS_CACHE As ObjectCache
        Get
            Return _coreWSCache
        End Get
    End Property



    Public Sub New()

        Dim cacheConfig = New NameValueCollection()
        cacheConfig.Add("pollingInterval", "00:05:00")
        cacheConfig.Add("physicalMemoryLimitPercentage", "0")
        cacheConfig.Add("cacheMemoryLimitMegabytes", "200")

        _cache = New MemoryCache("globalCache", cacheConfig)
        _widgetCache = New MemoryCache("widgetCache", cacheConfig)
        _coreWSCache = New MemoryCache("corewsCache", cacheConfig)

    End Sub

    Public Shared Function Instance() As MemoryCacheFactory

        If (objSingleton Is Nothing) Then
            ' Thread Safe
            SyncLock (classLocker)
                If (objSingleton Is Nothing) Then

                    objSingleton = New MemoryCacheFactory()

                End If
            End SyncLock
        End If
        Return objSingleton

    End Function

    Public Sub Add_Cache_To_Collection(ByVal cache As ICacheableData)

        If IsNothing(cache) Then
            Return
        End If

        Dim tipo As String = cache.GetType().FullName
        If Not _cacheCollection.ContainsKey(tipo) Then
            _cacheCollection.TryAdd(tipo, cache)
        End If

    End Sub

    Public Function Elenca_Tuttto(Optional ByVal tipoCache As String = Nothing) As List(Of Agronica_Oggetto_Cache)

        If IsNothing(tipoCache) OrElse String.IsNullOrEmpty(tipoCache) Then
            Return Elenca_Tuttto()
        Else
            If _cacheCollection.ContainsKey(tipoCache) Then
                Dim listaOggettiInCache As New List(Of Agronica_Oggetto_Cache)
                listaOggettiInCache.AddRange(_cacheCollection(tipoCache).Dammi_Contenuto_Cache(tipoCache))
                Return listaOggettiInCache
            Else
                Return New List(Of Agronica_Oggetto_Cache)
            End If
        End If

    End Function

    Private Function Elenca_Tuttto() As List(Of Agronica_Oggetto_Cache)

        Dim listaOggettiInCache As New List(Of Agronica_Oggetto_Cache)
        For Each kvp In _cacheCollection
            listaOggettiInCache.AddRange(kvp.Value.Dammi_Contenuto_Cache(kvp.Key))
        Next
        Return listaOggettiInCache

    End Function

    Public Sub Pulisci_Tuttto(Optional ByVal tipoCache As String = Nothing)

        If IsNothing(tipoCache) OrElse String.IsNullOrEmpty(tipoCache) Then
            Pulisci_Tuttto()
        Else
            If _cacheCollection.ContainsKey(tipoCache) Then
                _cacheCollection(tipoCache).Pulisci_Cache()
            End If
        End If

    End Sub

    Private Sub Pulisci_Tuttto()

        For Each kvp In _cacheCollection
            kvp.Value.Pulisci_Cache()
        Next

    End Sub

    Public Sub PulisciElemento(ByVal chiave As String, ByVal tipoCache As String)

        If String.IsNullOrEmpty(chiave) OrElse String.IsNullOrEmpty(tipoCache) Then
            Return
        End If

        For Each kvp In _cacheCollection
            If kvp.Value.CACHE_KEY_PREFIX.Equals(tipoCache) Then
                kvp.Value.Rimuovi_Elemento_Dalla_Cache(chiave)
                Exit For
            End If
        Next


    End Sub

End Class