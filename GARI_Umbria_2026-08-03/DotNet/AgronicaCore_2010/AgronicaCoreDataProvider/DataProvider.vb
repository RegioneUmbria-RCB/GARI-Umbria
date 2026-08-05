Imports System.Data.OleDb
Imports System.Configuration
Imports System.Data.Common
Imports System.Data.SqlClient
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Reflection
Imports System.Collections.Specialized
Imports System.Security.Cryptography
Imports System.Collections.Concurrent
Imports System.Runtime.Caching

Public Class DataProvider
    Inherits AgronicaCoreDataProvider.LogProvider


    Private _parametrizzatore As IParametrizzatore = Nothing
    Private _factory As DataProviderFactory = Nothing

    Private _creazioneParametriInLine As Boolean = True
    Private _sqlDiretto As Boolean = False
    Private _mantieniParametri As Boolean
    Public Property CreazioneParametriInLine() As Boolean
        Get
            Return _creazioneParametriInLine
        End Get
        Set(ByVal value As Boolean)
            _creazioneParametriInLine = value
        End Set
    End Property

    Public Property SqlDiretto() As Boolean
        Get
            Return _sqlDiretto
        End Get
        Set(ByVal value As Boolean)
            _sqlDiretto = value
        End Set
    End Property

    Public WriteOnly Property MantieniParametri() As Boolean
        Set(ByVal value As Boolean)
            _mantieniParametri = value
        End Set
    End Property

    Public ReadOnly Property CopiaParametri() As Dictionary(Of Int32, AgroDBParametro)
        Get
            If Not IsNothing(_parametrizzatore) Then
                Return _parametrizzatore.CopiaParametri()
            End If

            Return New Dictionary(Of Integer, AgroDBParametro)
        End Get
    End Property


    Public Sub New()
        _factory = DataProviderFactory.Instance
        '_parametrizzatore = New Parametrizzatore(_factory.ConfigrurazioneLogProvider)
        _parametrizzatore = DataProviderFactory.Instance.Parametrizzatore(_factory.ConfigrurazioneLogProvider)
    End Sub

    <Obsolete("Usare il metodo VersioneSqlServer_Major")>
    Public Function VersioneSqlServer_anno(objParametri As AgronicaCoreParametri) As Integer

        Return DataProviderFactory.Instance.Provider().VersioneSqlServer_anno(objParametri)

    End Function

    Public Function VersioneSqlServer_Major(objParametri As AgronicaCoreParametri) As Integer
        Return DataProviderFactory.Instance.Provider().VersioneSqlServer_Major(objParametri)
    End Function

    Public Function LivelloCompatibilita(objParametri As AgronicaCoreParametri) As Integer
        Dim dbName = objParametri.Recupera_NomeDB()
        If _factory.cacheCompatibilita.Contains(dbName) Then
            Return CInt(_factory.cacheCompatibilita.Get(dbName))
        End If

        Dim cp As New CacheItemPolicy
        Dim val = DataProviderFactory.Instance.Provider().LivelloCompatibilita(objParametri)
        _factory.cacheCompatibilita.Set(dbName, val, cp)

        Return val

    End Function


    '##############################################################################################
    Public Function EseguiQuery_Lettura(
                                        ByRef objConnessione As DbConnection,
                                        ByVal StringaConnessione As String,
                                        ByVal StringaSQL As String,
                                        ByVal DirectoryLOG As String,
                                        ByVal FileLOG As String,
                                        ByVal IdentificatoreUtente As String,
                                        ByVal NomeRoutine As String,
                                        Optional ByVal objTransazione As DbTransaction = Nothing
                                        ) As DataTable

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro)
        Return provider.EseguiQuery_Lettura(objConnessione,
                                            DataProviderFactory.Instance.AggiustaStringaDiConnessione(StringaConnessione),
                                            StringaSQL,
                                            DirectoryLOG,
                                            FileLOG,
                                            IdentificatoreUtente,
                                            NomeRoutine,
                                            objTransazione)

    End Function


    '##############################################################################################
    Public Function EseguiQuery_Lettura(
                                        ByRef objConnessione As DbConnection,
                                        ByRef objTransazione As DbTransaction,
                                        ByVal StringaConnessione As String,
                                        ByVal StringaSQL As String,
                                        ByVal DirectoryLOG As String,
                                        ByVal FileLOG As String,
                                        ByVal IdentificatoreUtente As String,
                                        ByVal NomeRoutine As String
                                        ) As DataTable

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro)
        Return provider.EseguiQuery_Lettura(
            objConnessione,
            objTransazione,
            DataProviderFactory.Instance.AggiustaStringaDiConnessione(StringaConnessione),
            StringaSQL,
            DirectoryLOG,
            FileLOG,
            IdentificatoreUtente,
            NomeRoutine
        )


    End Function


    '##############################################################################################
    Public Function EseguiQuery_Lettura_XML(
                                        ByRef objParametri As AgronicaCoreParametri,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String
                                        ) As String

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro)
        Return provider.EseguiQuery_Lettura_XML(
            objParametri,
            StringaSQL,
            NomeRoutine
        )

    End Function


    '##############################################################################################
    Public Function EseguiQuery_Lettura_jSon(
                                        ByRef objParametri As AgronicaCoreParametri,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String
                                        ) As String

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro)
        Return provider.EseguiQuery_Lettura_jSon(
            objParametri,
            StringaSQL,
            NomeRoutine
        )

    End Function

    '##############################################################################################
    Public Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String,
                                            Optional chiamaScriviLog As Boolean = True
                                        ) As DataTable

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro, identificatoreMetodo)
        provider.SqlDiretto = SqlDiretto

        Return provider.EseguiQuery_Lettura(
                objParametri,
                StringaSQL,
                NomeRoutine,
                chiamaScriviLog:=chiamaScriviLog
            )

    End Function


    '##############################################################################################
    Public Function EseguiQuery_Lettura(
                                        ByRef objParametri As AgronicaCoreParametri,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String,
                                        ByRef DataSet2Fill As DataSet,
                                        ByVal strNomeDtNelDS As String
                                        ) As Boolean

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro)
        provider.SqlDiretto = SqlDiretto
        Return provider.EseguiQuery_Lettura(
            objParametri,
            StringaSQL,
            NomeRoutine,
            DataSet2Fill,
            strNomeDtNelDS
        )

    End Function


    '##############################################################################################
    Public Function EseguiQuery_Lettura(
                                        ByRef StringaConnessione As String,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String
                                        ) As DataTable

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro)
        Return provider.EseguiQuery_Lettura(
            DataProviderFactory.Instance.AggiustaStringaDiConnessione(StringaConnessione),
            StringaSQL,
            NomeRoutine
        )

    End Function

    Public Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri,
                                        StringaSQL As String,
                                        parametriCommand As Dictionary(Of String, Object),
                                        NomeRoutine As String) As DataTable

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro)
        Return provider.EseguiQuery_Lettura(
            objParametri,
            StringaSQL,
            parametriCommand,
            NomeRoutine
        )

    End Function

    Public Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, ByVal parametriCommand As List(Of DbParameter), NomeRoutine As String) As DataTable

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro)
        Return provider.EseguiQuery_Lettura(
           objParametri,
           StringaSQL,
           parametriCommand,
           NomeRoutine
       )

    End Function

    Public Function EseguiQuery_Scrittura(
                                        ByRef objConnessione As DbConnection,
                                        ByRef objTransazione As DbTransaction,
                                        ByVal StringaConnessione As String,
                                        ByVal StringaSQL As String,
                                        ByVal DirectoryLOG As String,
                                        ByVal FileLOG As String,
                                        ByVal IdentificatoreUtente As String,
                                        ByVal NomeRoutine As String
                                        ) As Boolean

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro)
        Return provider.EseguiQuery_Scrittura(
            objConnessione,
            objTransazione,
            _factory.AggiustaStringaDiConnessione(StringaConnessione),
            StringaSQL,
            DirectoryLOG,
            FileLOG,
            IdentificatoreUtente,
            NomeRoutine
        )

    End Function


    '##############################################################################################
    'fatto per apofruit e scrivere su siagr, non testato sul gas o altro
    Public Function EseguiQuery_Scrittura(
                                        ByRef stringaConnessione As String,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String
                                        ) As Boolean

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro)
        Return provider.EseguiQuery_Scrittura(
            _factory.AggiustaStringaDiConnessione(stringaConnessione),
            StringaSQL,
            NomeRoutine
        )

    End Function



    '##############################################################################################
    Public Function EseguiQuery_ScritturaNum(
                                        ByRef objParametri As AgronicaCoreParametri,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String,
                                        ByRef NumeroRecordInteressati As Integer) As Boolean

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro)
        Return provider.EseguiQuery_ScritturaNum(
            objParametri,
            StringaSQL,
            NomeRoutine,
            NumeroRecordInteressati
        )

    End Function


    Public Function EseguiQuery_Scrittura(
                                    ByRef objParametri As AgronicaCoreParametri,
                                    ByVal StringaSQL As String,
                                    ByVal NomeRoutine As String
                                    ) As Boolean

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro)
        Return provider.EseguiQuery_Scrittura(
            objParametri,
            StringaSQL,
            NomeRoutine
        )

    End Function

    Public Function EseguiQuery_Scrittura_Param(
                                               ByRef objParametri As AgronicaCoreParametri,
                                               StringaSQL As String,
                                               NomeRoutine As String,
                                               parametriCommand As List(Of DbParameter)) As Boolean

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro)
        Return provider.EseguiQuery_Scrittura_Param(
            objParametri,
            StringaSQL,
            NomeRoutine,
            parametriCommand
        )

    End Function

    Function EseguiQuery_Scrittura_Param(ByRef objParametri As AgronicaCoreParametri,
                                         ByRef objConnessione As DbConnection,
                                         ByRef objTransazione As DbTransaction,
                                         StringaConnessione As String,
                                         StringaSQL As String,
                                         NomeRoutine As String,
                                         parametriCommand As List(Of DbParameter)) As Boolean

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro)
        Return provider.EseguiQuery_Scrittura_Param(
            objParametri,
            objConnessione,
            objTransazione,
            StringaConnessione,
            StringaSQL,
            NomeRoutine,
            parametriCommand)

    End Function


    Public Function NomeDataBase_FromConnessione_ATTENZIONE_NON_USARE_PER_COMPATIBILITA_SUPERSERVER(ByVal strPathxFileIni As String,
                                                       ByVal strKey As String,
                                                       Optional ByVal strNomeAttributoDB As String = "DbParam") _
                                                       As String
        Return _factory.Provider().NomeDataBase_FromConnessione_ATTENZIONE_NON_USARE_PER_COMPATIBILITA_SUPERSERVER(
            strPathxFileIni,
            strKey,
            strNomeAttributoDB
        )

    End Function

    Public Function NomeDataBase_FromStringaConnessione(ByVal StringaConnessione As String) _
                                                   As String

        Return _factory.Provider().NomeDataBase_FromStringaConnessione(StringaConnessione)

    End Function


    'ricerca la connessione nel file ini  o nel superserver  in base alla k passata (se intero allora su superserver)
    Public Function FindConnessione_Su_Ini_O_Superserver(ByVal strPath2Ini As String, ByVal strKey As String, Optional objParametriSuperServer As AgronicaCoreParametri = Nothing) As String

        Return _factory.Provider().FindConnessione_Su_Ini_O_Superserver(
            strPath2Ini,
            strKey,
            objParametriSuperServer
        )

    End Function

    'ricerca nel file ini la chiave x la connessione in base alla k passata
    Public Function FindIniConnessioni(ByVal strPath2Ini As String, ByVal strKey As String) As String

        Return _factory.Provider().FindIniConnessioni(
            strPath2Ini, strKey
        )

    End Function


    ' apre la connessione
    Public Function ApriConnessione(ByRef objParametri As AgronicaCoreParametri, ByRef strErr As String) _
                                    As DbConnection

        Return _factory.Provider().ApriConnessione(
            objParametri,
            strErr
        )

    End Function

    '###############################################################################
    Public Function EseguiQuery_InsertParametrizzata(ByRef objParametri As AgronicaCoreParametri,
                                               ByVal Nome_Tabella As String,
                                               ByVal strCampi As String,
                                               ByVal strValori As String) As Boolean

        Return _factory.Provider().EseguiQuery_InsertParametrizzata(
            objParametri,
            Nome_Tabella,
            strCampi,
            strValori
        )

    End Function


    '###########################################################################################################
    Public Function ConnectToAccess(ByVal DataSource As String) As OleDb.OleDbConnection

        Return _factory.Provider().ConnectToAccess(DataSource)

    End Function

    '#################################################################################################
    Public Function Scrivi_Dati_SuAccess(ByVal DataSource As String,
                                         ByVal StrSQL As String,
                                         ByRef strErr As String
                                         ) As Object

        Return _factory.Provider().Scrivi_Dati_SuAccess(
            DataSource,
            StrSQL,
            strErr
        )


    End Function

    Public Function EseguiQuery_Scrittura_ParamVarBinary(
                                    ByRef objParametri As AgronicaCoreParametri,
                                    ByVal StringaSQL As String,
                                    ByVal NomeRoutine As String,
                                    ByVal CmdParameters As Dictionary(Of String, Byte())
                                    ) As Boolean

        Dim identificatoreMetodo As String = String.Empty
        Dim parametri = OttieniParametri(identificatoreMetodo)
        Dim orderByeFiltro = OttienyOrderByEFiltroAggiuntivo()
        If _mantieniParametri Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                _parametrizzatore.SettaCopiaParametri(parametri)
            End If
        End If

        Dim provider = _factory.Provider(parametri, orderByeFiltro)
        Return provider.EseguiQuery_Scrittura_ParamVarBinary(
            objParametri,
            StringaSQL,
            NomeRoutine,
            CmdParameters
        )

    End Function

    '##############################################################################################
    Protected Function Agro_SQL_SaveText(ByVal Testo As String, Optional ByVal creaParametroSql As Boolean = True) As String

        If creaParametroSql Then
            creaParametroSql = _factory.ParametrizzaQuery AndAlso Me.CreazioneParametriInLine
        End If

        Return _parametrizzatore.Agro_SQL_SaveText(Testo, creaParametroSql)

    End Function

    Protected Function Agro_SQL_SaveText_UNICODE(ByVal Testo As String, ByVal par As String, Optional ByVal creaParametroSql As Boolean = True) As String

        If creaParametroSql Then
            creaParametroSql = _factory.ParametrizzaQuery AndAlso Me.CreazioneParametriInLine
        End If

        Return _parametrizzatore.Agro_SQL_SaveText_UNICODE(Testo, par, creaParametroSql)

    End Function

    Protected Function Agro_SQL_SaveText_NULL(ByVal item As Object, Optional ByVal creaParametroSql As Boolean = True) As String

        If creaParametroSql Then
            creaParametroSql = _factory.ParametrizzaQuery AndAlso Me.CreazioneParametriInLine
        End If
        Return _parametrizzatore.Agro_SQL_SaveText_NULL(item, creaParametroSql)

    End Function

    ''' <param name="clausolaIN">Stringa contenente un elenco di valori separati da virgola. In caso i valori siano
    ''' stringhe, non è necessario che siano compresi tra apici, tuttavia è bene valorizzare a <tt>True</tt> il
    ''' parametro <tt>valoriStringa</tt>.</param>
    ''' <param name="valoriStringa">Se impostato a <tt>True</tt>, assicura che i valori siano compresi tra apici a
    ''' delimitare inizio e fine della stringa.</param>
    Protected Function Agro_SQL_Save_Clausola_IN(ByVal clausolaIN As String, Optional ByVal valoriStringa As Boolean = False, Optional ByVal creaParametriSql As Boolean = True) As String

        If creaParametriSql Then
            creaParametriSql = _factory.ParametrizzaQuery AndAlso Me.CreazioneParametriInLine
        End If
        Return _parametrizzatore.Agro_SQL_Save_Clausola_IN(clausolaIN, valoriStringa, creaParametriSql)

    End Function

    Protected Function Agro_SQL_SaveNum(ByVal StringaNumero As String, Optional ByVal creaParametroSql As Boolean = True) As String

        If creaParametroSql Then
            creaParametroSql = _factory.ParametrizzaQuery AndAlso Me.CreazioneParametriInLine
        End If
        Return _parametrizzatore.Agro_SQL_SaveNum(StringaNumero, creaParametroSql)

    End Function

    '##############################################################################################
    'in realtà fa cast a datetime
    Protected Function Agro_SQL_SaveDate(ByVal DataItaliana As Date, Optional ByVal creaParametroSql As Boolean = True) As String

        If creaParametroSql Then
            creaParametroSql = _factory.ParametrizzaQuery AndAlso Me.CreazioneParametriInLine
        End If
        Return _parametrizzatore.Agro_SQL_SaveDate(DataItaliana, creaParametroSql)

    End Function

    Protected Function Agro_SQL_SaveDateTime(ByVal DataOraItaliana As DateTime, Optional ByVal creaParametroSql As Boolean = True) As String

        If creaParametroSql Then
            creaParametroSql = _factory.ParametrizzaQuery AndAlso Me.CreazioneParametriInLine
        End If
        Return _parametrizzatore.Agro_SQL_SaveDateTime(DataOraItaliana, creaParametroSql)

    End Function

    Protected Function Agro_SQL_SaveDateTime_NULL(ByVal item As Object, Optional ByVal creaParametroSql As Boolean = True) As String

        If creaParametroSql Then
            creaParametroSql = _factory.ParametrizzaQuery AndAlso Me.CreazioneParametriInLine
        End If
        Return _parametrizzatore.Agro_SQL_SaveDateTime_NULL(item, creaParametroSql)

    End Function

    Protected Function Agro_SQL_Save_xFiltroAggiuntivo(
                                                      ByVal filtro As String,
                                                      Optional ByVal creaParametriSql As Boolean = True,
                                                      Optional ByVal objParametri As AgronicaCoreParametri = Nothing) As String

        If creaParametriSql Then
            creaParametriSql = _factory.ParametrizzaQuery
        End If

        Dim filtroRipulito = filtro.ToOrigin(objParametri).Replace(Environment.NewLine, " ")
        filtroRipulito = filtroRipulito.Replace(vbLf, " ")

        If _factory.TipoProvider = enum_DataProvidersType.OleDbProvider Then
            Return filtro
        End If

        Dim PREFISSO_CHIAVE_CACHE As String = "xFiltroAggiuntivo"

        _parametrizzatore.FiltroAggiuntivoOriginale = filtro

        ' provo a leggere la configurazione della cache
        Dim cfgEstesa = ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri)
        ' se la lettura è andata a buon fine e la cache non è ancora stata inizializzata la inizializzo
        If cfgEstesa IsNot Nothing AndAlso IsNothing(_factory.CacheFiltroAggiuntivo) Then
            _factory.CacheFiltroAggiuntivo = New CachedFiltroAggiuntivo(cfgEstesa)
            MemoryCacheFactory.Instance.Add_Cache_To_Collection(_factory.CacheFiltroAggiuntivo)
            _factory.ConfigurazioneEstesa = cfgEstesa
        End If

        Dim cache = _factory.CacheFiltroAggiuntivo
        Dim filtroCacheato As FiltroAggiuntivoCacheEntry = Nothing

        If Not IsNothing(cache) Then
            filtroCacheato = cache.Leggi(filtroRipulito, objParametri)
        End If

        If IsNothing(filtroCacheato) Then

            Dim filtroAggiuntivo As String = _parametrizzatore.Agro_SQL_Save_xFiltroAggiuntivo(filtroRipulito, creaParametriSql, Nothing, objParametri)
            _parametrizzatore.FiltroAggiuntivoManipolato = filtroAggiuntivo

            If cache IsNot Nothing Then
                ' inserisco il filtro parsato nella cache
                cache.Scrivi(filtroRipulito,
                      New FiltroAggiuntivoCacheEntry With
                            {
                                .FiltroAggiuntivoOriginale = filtroRipulito,
                                .FiltroAggiuntivoParsato = filtroAggiuntivo,
                                .CreaParametri = creaParametriSql,
                                .IsOrderBy = False
                            },
                            objParametri
                            )
            End If

            Return filtroAggiuntivo

        Else
            creaParametriSql = filtroCacheato.CreaParametri
            Return _parametrizzatore.Agro_SQL_Save_xFiltroAggiuntivo_Semplificato(filtroRipulito, creaParametriSql, Nothing, objParametri)
        End If

    End Function

    Protected Function Agro_SQL_Save_xOrderBy(ByVal filtro As String, Optional ByVal objParametri As AgronicaCoreParametri = Nothing) As String

        Dim orderByRipulito = filtro.ToOrigin(objParametri).Replace(Environment.NewLine, " ")
        orderByRipulito = orderByRipulito.Replace(vbLf, " ")

        If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
            Return filtro
        End If

        If String.IsNullOrEmpty(filtro) Then
            Return filtro
        End If

        _parametrizzatore.OrderByOriginale = filtro

        ' provo a leggere la configurazione della cache
        Dim cfgEstesa = ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri)
        ' se la lettura è andata a buon fine e la cache non è ancora stata inizializzata la inizializzo
        If cfgEstesa IsNot Nothing AndAlso IsNothing(DataProviderFactory.Instance.CacheFiltroAggiuntivo) Then
            DataProviderFactory.Instance.CacheFiltroAggiuntivo = New CachedFiltroAggiuntivo(cfgEstesa)
            MemoryCacheFactory.Instance.Add_Cache_To_Collection(DataProviderFactory.Instance.CacheFiltroAggiuntivo)
            DataProviderFactory.Instance.ConfigurazioneEstesa = cfgEstesa
        End If

        ' Controllo se esiste in cache
        Dim cache = DataProviderFactory.Instance.CacheFiltroAggiuntivo
        Dim orderByCacheato As FiltroAggiuntivoCacheEntry = Nothing

        If cache IsNot Nothing Then
            orderByCacheato = cache.Leggi(orderByRipulito, objParametri)
        End If

        If IsNothing(orderByCacheato) Then
            Dim orderBy = _parametrizzatore.Agro_SQL_Save_xOrderBy(orderByRipulito, objParametri)
            _parametrizzatore.OrderByManipolato = orderBy

            If cache IsNot Nothing Then
                ' inserisco il filtro parsato nella cache
                cache.Scrivi(orderByRipulito,
                New FiltroAggiuntivoCacheEntry With
                                {
                                    .FiltroAggiuntivoParsato = orderBy,
                                    .IsOrderBy = True
                                },
                                objParametri
                                )
            End If

            Return orderBy
        Else
            Return filtro
        End If

    End Function

    Private Function BypassaLog() As Boolean

        'Dim st As New StackTrace
        'Dim mi As MethodBase = st.GetFrame(2).GetMethod()
        'If mi.IsDefined(GetType(DataProviderBypassLogAttribute), False) Then
        '    Return True
        'End If

        Return False

    End Function

    Private Function OttieniParametri(ByRef identificatoreMetodo As String) As Dictionary(Of Int32, AgroDBParametro)

        Dim usaParametriInjected As Boolean = False
        identificatoreMetodo = String.Empty

        If Not _factory.ParametrizzaQuery Then
            Return Nothing
        End If

        Dim st As New StackTrace
        Dim mi As MethodBase = st.GetFrame(2).GetMethod()
        If mi.IsDefined(GetType(DataProviderInjectParameterAttribute), False) Then
            Dim attr = DirectCast(mi.GetCustomAttributes(GetType(DataProviderInjectParameterAttribute), False).FirstOrDefault, DataProviderInjectParameterAttribute)
            Dim inject As Boolean = attr.UsaInjectionParametri
            If inject Then
                usaParametriInjected = True
                identificatoreMetodo = attr.IdentificatoreMetodo.ToString
            End If
        End If

        If Not usaParametriInjected Then
            Return _parametrizzatore.Parametri
        Else
            Dim paramInjected = _factory.ParametriInjected
            If paramInjected IsNot Nothing AndAlso paramInjected.ContainsKey(identificatoreMetodo) Then
                Return paramInjected(identificatoreMetodo)
            End If
        End If

        Return Nothing

    End Function

    Private Function IsCacheable(ByRef attributi As CacheableAttribute) As Boolean

        attributi = Nothing

        Dim st As New StackTrace
        Dim mi As MethodBase = st.GetFrame(2).GetMethod()
        If mi.IsDefined(GetType(CacheableAttribute), False) Then
            Dim attr = DirectCast(mi.GetCustomAttributes(GetType(CacheableAttribute), False).FirstOrDefault, CacheableAttribute)
            If attr.Cacheable Then
                attributi = attr
                Return True
            Else
                Return False
            End If
        End If

        Return False

    End Function

    Private Function OttienyOrderByEFiltroAggiuntivo() As OrderByFiltroAggiuntivo

        If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
            Return Nothing
        End If
        If Not _factory.ParametrizzaQuery Then
            Return Nothing
        End If

        Return New OrderByFiltroAggiuntivo With
            {
                .FiltroAggiuntivoOriginale = _parametrizzatore.FiltroAggiuntivoOriginale,
                .FiltroAggiuntivoManipolato = _parametrizzatore.FiltroAggiuntivoManipolato,
                .OrderByOriginale = _parametrizzatore.OrderByOriginale,
                .OrderByManipolato = _parametrizzatore.OrderByManipolato
            }

    End Function

    Public Sub SettaParametriPrecedenti(ByVal parametriPrecedenti As Dictionary(Of Integer, AgroDBParametro))
        If _parametrizzatore IsNot Nothing Then

            If parametriPrecedenti IsNot Nothing AndAlso parametriPrecedenti.Any Then

                _parametrizzatore.Parametri.Clear()
                For Each kvp In parametriPrecedenti
                    _parametrizzatore.Parametri.Add(kvp.Key, kvp.Value)
                Next
            End If

        End If
    End Sub

    Public Sub SvuotaTuttiIParametri()

        If _parametrizzatore IsNot Nothing Then
            If _parametrizzatore.Parametri IsNot Nothing Then
                _parametrizzatore.Parametri.Clear()
            End If

            If _parametrizzatore.CopiaParametri IsNot Nothing Then
                _parametrizzatore.CopiaParametri.Clear()
            End If

        End If

    End Sub

    Public Function DammiParametriCollezionati() As Dictionary(Of Integer, AgroDBParametro)

        If _parametrizzatore IsNot Nothing Then
            Dim copiaParametri = New Dictionary(Of Integer, AgroDBParametro)
            For Each kvp In _parametrizzatore.Parametri
                copiaParametri.Add(kvp.Key, kvp.Value)
            Next
            Return copiaParametri
        End If

        Return Nothing

    End Function

    Public Function Parametrizza(ByRef stringaSql As String, ByRef parametriOutput As List(Of SqlParameter)) As Boolean

        Dim retVal As Boolean = False

        parametriOutput = New List(Of SqlParameter)
        If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.SqlDataProvider Then

            Dim parametriInput = OttieniParametri(String.Empty)
            If parametriInput IsNot Nothing AndAlso parametriInput.Any Then
                Dim provider = New SqlDataProvider(parametriInput)
                retVal = provider.Parametrizza(stringaSql, parametriOutput, Nothing)
            Else
                stringaSql = stringaSql.ToOrigin
            End If
        Else
            stringaSql = stringaSql.ToOrigin
        End If

        Return retVal

    End Function

End Class

Public Class DataProviderDbLogFactory

    Private Shared objSingleton As DataProviderDbLogFactory
    Private Shared classLocker As New Object()
    Private _hashLogDict = New ConcurrentDictionary(Of String, DbLogClass)

    Public ReadOnly Property HashDict As ConcurrentDictionary(Of String, DbLogClass)
        Get
            Return _hashLogDict
        End Get
    End Property


    Public Sub New()

        _hashLogDict = New ConcurrentDictionary(Of String, DbLogClass)

    End Sub

    Public Shared Function Instance() As DataProviderDbLogFactory

        If (objSingleton Is Nothing) Then
            ' Thread Safe
            SyncLock (classLocker)
                If (objSingleton Is Nothing) Then

                    objSingleton = New DataProviderDbLogFactory()

                End If
            End SyncLock
        End If
        Return objSingleton

    End Function

End Class

Public Class DataProviderFactory
    Implements ISqlSave

    Private Const DefaultProviderTypeName As String = "SqlDataProvider"

    Private _tipoDataProviderCorrente As String = DefaultProviderTypeName
    Private _tipoProvider As enum_DataProvidersType = enum_DataProvidersType.Undefined
    Private _configrurazioneLogProvider As ConfigurazioneLogProviderEsteso
    Private _cacheFiltroAggiuntivo As CachedFiltroAggiuntivo = Nothing
    Private _cacheQueryInErrore As CachedQueryInErrore = Nothing
    Private _cacheGenerale As CachedDataProvider = Nothing
    Private _parametrizzaQuery As Boolean = True
    Private _parametrizzatore As IParametrizzatore = Nothing
    Private _parametriInjected As Dictionary(Of String, Dictionary(Of Int32, AgroDBParametro)) = Nothing
    Private _configurazioneEstesaSqlProvider As ConfigurazioneEstesaSqlProvider = Nothing
    Private _hashLogDict As Dictionary(Of String, DbLogClass) = Nothing
    Private Shared classLocker As New Object()
    Private Shared objSingleton As DataProviderFactory
    Private _tipoParametrizzatore As ParametrizzatoreType = ParametrizzatoreType.PARAMETRIZZATORE_NEW

    Private _cacheCompatibilita As MemoryCache

    Public ReadOnly Property ConfigrurazioneLogProvider As ConfigurazioneLogProviderEsteso
        Get
            Return _configrurazioneLogProvider
        End Get
    End Property

    Public ReadOnly Property TipoProvider As enum_DataProvidersType
        Get
            Return _tipoProvider
        End Get
    End Property

    Public ReadOnly Property TipoParametrizzatore As ParametrizzatoreType
        Get
            Return _tipoParametrizzatore
        End Get
    End Property

    Public ReadOnly Property ParametrizzaQuery As Boolean
        Get
            Return _parametrizzaQuery
        End Get
    End Property

    Public ReadOnly Property ParametriInjected() As Dictionary(Of String, Dictionary(Of Int32, AgroDBParametro))
        Get
            Return _parametriInjected
        End Get
    End Property

    Public Property HashDict As Dictionary(Of String, DbLogClass)
        Get
            Return _hashLogDict
        End Get
        Set(value As Dictionary(Of String, DbLogClass))
            _hashLogDict = value
        End Set
    End Property

    Public Property CacheFiltroAggiuntivo() As CachedFiltroAggiuntivo
        Get
            Return _cacheFiltroAggiuntivo
        End Get
        Set(value As CachedFiltroAggiuntivo)
            _cacheFiltroAggiuntivo = value
        End Set
    End Property

    Public Property CacheQueryInErrore() As CachedQueryInErrore
        Get
            Return _cacheQueryInErrore
        End Get
        Set(value As CachedQueryInErrore)
            _cacheQueryInErrore = value
        End Set
    End Property

    Public Property CacheGenerale() As CachedDataProvider
        Get
            Return _cacheGenerale
        End Get
        Set(value As CachedDataProvider)
            _cacheGenerale = value
        End Set
    End Property


    Public Property ConfigurazioneEstesa() As ConfigurazioneEstesaSqlProvider
        Get
            Return _configurazioneEstesaSqlProvider
        End Get
        Set(value As ConfigurazioneEstesaSqlProvider)
            _configurazioneEstesaSqlProvider = value
        End Set
    End Property


    Public Property cacheCompatibilita() As MemoryCache
        Get
            Return _cacheCompatibilita
        End Get
        Set(value As MemoryCache)
            _cacheCompatibilita = value
        End Set
    End Property


    Private Sub New()
        _cacheCompatibilita = New MemoryCache("livelloCompatibilitaCache")
        _parametriInjected = New Dictionary(Of String, Dictionary(Of Integer, AgroDBParametro))
    End Sub

    Private Sub New(ByVal dataProviderType As String, ByVal parametrizzaQuery As Boolean, ByVal tipoParametrizzatore As ParametrizzatoreType)

        Me.New
        _tipoDataProviderCorrente = dataProviderType
        _parametrizzaQuery = parametrizzaQuery
        _tipoParametrizzatore = tipoParametrizzatore
        '_hashLogDict = New Dictionary(Of String, DbLogClass)

        Select Case dataProviderType.ToLowerInvariant
            Case "OleDbProvider".ToLowerInvariant()
                _tipoProvider = enum_DataProvidersType.OleDbProvider
                _parametrizzaQuery = False
            Case "SqlDataProvider".ToLowerInvariant()
                _tipoProvider = enum_DataProvidersType.SqlDataProvider
            Case Else
                _tipoProvider = enum_DataProvidersType.Undefined
        End Select

        '_parametrizzatore = New Parametrizzatore(Nothing)
        _parametrizzatore = Parametrizzatore(Nothing)
        _cacheFiltroAggiuntivo = Nothing
        _cacheCompatibilita = New MemoryCache("livelloCompatibilitaCache")
    End Sub

    Public Shared Function Instance() As DataProviderFactory

        If (objSingleton Is Nothing) Then
            ' Thread Safe
            SyncLock (classLocker)
                If (objSingleton Is Nothing) Then

                    Dim tipoDataProvider As String = DefaultProviderTypeName
                    If Not IsNothing(ConfigurationManager.AppSettings("DataProviderType")) AndAlso Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("DataProviderType").ToString()) Then
                        tipoDataProvider = ConfigurationManager.AppSettings("DataProviderType").ToString()
                    End If

                    Dim parametrizzaQuery As Boolean = True
                    If Not IsNothing(ConfigurationManager.AppSettings("UsaSqlParameters")) AndAlso Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("UsaSqlParameters").ToString()) Then
                        parametrizzaQuery = Convert.ToBoolean(ConfigurationManager.AppSettings("UsaSqlParameters"))
                    End If

                    Dim tipoParametrizzatore As ParametrizzatoreType = ParametrizzatoreType.PARAMETRIZZATORE_NEW
                    If Not IsNothing(ConfigurationManager.AppSettings("DataProviderParametrizerType")) AndAlso Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("DataProviderParametrizerType").ToString()) Then
                        tipoParametrizzatore = Convert.ToInt32(ConfigurationManager.AppSettings("DataProviderParametrizerType"))
                    End If

                    objSingleton = New DataProviderFactory(tipoDataProvider, parametrizzaQuery, tipoParametrizzatore)

                    If Debugger.IsAttached Then
                        MemoryCacheFactory.Instance.EnebaleCacheDataProvider = False
                    End If

                    MemoryCacheFactory.Instance.Add_Cache_To_Collection(New CachedDataProvider)

                End If
            End SyncLock
        End If
        Return objSingleton

    End Function

    Public Function Parametrizzatore(ByVal configurazioneLogProviderEsteso As ConfigurazioneLogProviderEsteso) As IParametrizzatore

        Select Case _tipoParametrizzatore
            Case ParametrizzatoreType.PARAMETRIZZATORE
                Return New Parametrizzatore(configurazioneLogProviderEsteso)
            Case ParametrizzatoreType.PARAMETRIZZATORE_NEW
                Return New ParametrizzatoreNew(configurazioneLogProviderEsteso)
            Case Else
                Return New Parametrizzatore(configurazioneLogProviderEsteso)
        End Select

    End Function

    Public Function Provider(
                            Optional ByRef parametri As Dictionary(Of Int32, AgroDBParametro) = Nothing,
                            Optional ByVal orderByeFiltro As OrderByFiltroAggiuntivo = Nothing,
                            Optional identificatoreMetodo As String = "") As IDataProvider

        Dim dataProvider As IDataProvider = Nothing

        If Not Me.ParametrizzaQuery Then
            parametri = Nothing
        End If

        Select Case _tipoProvider
            Case enum_DataProvidersType.OleDbProvider
                If parametri Is Nothing Then
                    dataProvider = New OleDbDataProvider
                Else
                    dataProvider = New OleDbDataProvider(New Dictionary(Of Integer, AgroDBParametro)(parametri))
                    RimuoviParametri(parametri, identificatoreMetodo)
                End If

            Case enum_DataProvidersType.SqlDataProvider
                If parametri Is Nothing Then
                    dataProvider = New SqlDataProvider
                Else
                    Dim sqlDataProvider = New SqlDataProvider(New Dictionary(Of Integer, AgroDBParametro)(parametri))
                    sqlDataProvider.SettaOrderBYeFiltroAggiuntivo(orderByeFiltro)
                    dataProvider = sqlDataProvider
                    RimuoviParametri(parametri, identificatoreMetodo)
                End If

            Case Else
                Throw New NotImplementedException("Provider dati non supportato")
        End Select

        Return dataProvider

    End Function

    Public Function AggiustaStringaDiConnessione(ByVal stringaConnessione As String) As String

        Select Case _tipoProvider
            Case enum_DataProvidersType.OleDbProvider
                Return stringaConnessione
            Case enum_DataProvidersType.SqlDataProvider
                Return getConnectionStringFromOleToSql(stringaConnessione)
            Case Else
                Throw New NotImplementedException("Provider dati non supportato")
        End Select
    End Function

    Public Function CreaNuovaConnessione() As DbConnection

        Select Case _tipoProvider
            Case enum_DataProvidersType.OleDbProvider
                Return New OleDbConnection()
            Case enum_DataProvidersType.SqlDataProvider
                Return New SqlConnection()
            Case Else
                Throw New NotImplementedException("Provider dati non supportato")
        End Select

    End Function

    Public Function CreaNuovaConnessione(ByVal stringaConnessione As String) As DbConnection

        Select Case _tipoProvider
            Case enum_DataProvidersType.OleDbProvider
                Return New OleDbConnection(stringaConnessione)
            Case enum_DataProvidersType.SqlDataProvider
                Return New SqlConnection(getConnectionStringFromOleToSql(stringaConnessione))
            Case Else
                Throw New NotImplementedException("Provider dati non supportato")
        End Select

    End Function
    Public Function CreaCommand() As DbCommand

        Select Case _tipoProvider
            Case enum_DataProvidersType.OleDbProvider
                Return New OleDbCommand()
            Case enum_DataProvidersType.SqlDataProvider
                Return New SqlCommand()
            Case Else
                Throw New NotImplementedException("Provider dati non supportato")
        End Select

    End Function

    Public Function CreaCommand(ByVal cmdText As String, ByVal connessione As DbConnection) As DbCommand

        Select Case _tipoProvider
            Case enum_DataProvidersType.OleDbProvider
                Return New OleDbCommand(cmdText, connessione)
            Case enum_DataProvidersType.SqlDataProvider
                Return New SqlCommand(cmdText, connessione)
            Case Else
                Throw New NotImplementedException("Provider dati non supportato")
        End Select

    End Function

    Public Function CreaParameter() As DbParameter

        Select Case _tipoProvider
            Case enum_DataProvidersType.OleDbProvider
                Return New OleDbParameter()
            Case enum_DataProvidersType.SqlDataProvider
                Return New SqlParameter()
            Case Else
                Throw New NotImplementedException("Provider dati non supportato")
        End Select

    End Function

    Public Function CreaParameter(ByVal name As String, ByVal value As Object) As DbParameter

        Select Case _tipoProvider
            Case enum_DataProvidersType.OleDbProvider
                Return New OleDbParameter(name.Replace("@", "?"), value)
            Case enum_DataProvidersType.SqlDataProvider
                Return New SqlParameter(name.Replace("?", "@"), value)
            Case Else
                Throw New NotImplementedException("Provider dati non supportato")
        End Select

    End Function

    Public Function CreaDataAdapter() As DbDataAdapter

        Select Case _tipoProvider
            Case enum_DataProvidersType.OleDbProvider
                Return New OleDbDataAdapter()
            Case enum_DataProvidersType.SqlDataProvider
                Return New SqlDataAdapter
            Case Else
                Throw New NotImplementedException("Provider dati non supportato")
        End Select

    End Function

    Public Function CreaDataAdapter(ByVal command As DbCommand) As DbDataAdapter

        Select Case _tipoProvider
            Case enum_DataProvidersType.OleDbProvider
                Return New OleDbDataAdapter(command)
            Case enum_DataProvidersType.SqlDataProvider
                Return New SqlDataAdapter(command)
            Case Else
                Throw New NotImplementedException("Provider dati non supportato")
        End Select

    End Function

    Public Function CreaDataAdapter(ByVal querySql As String, ByVal connessione As DbConnection) As DbDataAdapter

        Select Case _tipoProvider
            Case enum_DataProvidersType.OleDbProvider
                Return New OleDbDataAdapter(querySql, connessione)
            Case enum_DataProvidersType.SqlDataProvider
                Return New SqlDataAdapter(querySql, connessione)
            Case Else
                Throw New NotImplementedException("Provider dati non supportato")
        End Select

    End Function

    Public Function CreaCommandBuilder(ByVal adapter As DbDataAdapter) As DbCommandBuilder

        Select Case _tipoProvider
            Case enum_DataProvidersType.OleDbProvider
                Return New OleDbCommandBuilder(adapter)
            Case enum_DataProvidersType.SqlDataProvider
                Return New SqlCommandBuilder(adapter)
            Case Else
                Throw New NotImplementedException("Provider dati non supportato")
        End Select

    End Function


    Private Function getConnectionStringFromOleToSql(ByVal sConnessioneOle As String) As String

        Dim separator(1) As Char
        separator(0) = ";"c
        separator(1) = "="c

        Dim rval As String = ""

        Dim DaTenere As String = "server,data source,initial catalog,user id,password"

        Dim arrayFromString() As String = sConnessioneOle.Split(separator)
        For i As Integer = 0 To arrayFromString.Length - 2 Step 2
            If DaTenere.IndexOf(Trim(arrayFromString(i)).ToLower, 0) > -1 Then
                rval &= arrayFromString(i) & "=" & arrayFromString(i + 1) & ";"
            End If
        Next

        Return rval
    End Function

    Public Function Agro_SQL_SaveText(ByVal Testo As String, Optional creaParametroSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String Implements ISqlSave.Agro_SQL_SaveText

        If Not IsNothing(injectionGuid) Then
            creaParametroSql = True
        End If

        creaParametroSql = Me.ParametrizzaQuery
        Return _parametrizzatore.Agro_SQL_SaveText(Testo, creaParametroSql, injectionGuid)

    End Function

    Public Function Agro_SQL_SaveText_UNICODE(Testo As String, par As String, Optional creaParametroSql As Boolean = True, Optional injectionGuid As Guid = Nothing) As String Implements ISqlSave.Agro_SQL_SaveText_UNICODE

        If Not IsNothing(injectionGuid) Then
            creaParametroSql = True
        End If

        creaParametroSql = Me.ParametrizzaQuery
        Return _parametrizzatore.Agro_SQL_SaveText_UNICODE(Testo, par, creaParametroSql, injectionGuid)

    End Function
    Public Function Agro_SQL_SaveText_NULL(ByVal item As Object, Optional ByVal creaParametroSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String Implements ISqlSave.Agro_SQL_SaveText_NULL

        If Not IsNothing(injectionGuid) Then
            creaParametroSql = True
        End If

        creaParametroSql = Me.ParametrizzaQuery
        Return _parametrizzatore.Agro_SQL_SaveText_NULL(item, creaParametroSql, injectionGuid)

    End Function

    Public Function Agro_SQL_Save_Clausola_IN(ByVal clausolaIN As String, ByVal valoriStringa As Boolean, Optional ByVal creaParametriSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String Implements ISqlSave.Agro_SQL_Save_Clausola_IN

        If Not IsNothing(injectionGuid) Then
            creaParametriSql = True
        End If

        creaParametriSql = Me.ParametrizzaQuery
        Return _parametrizzatore.Agro_SQL_Save_Clausola_IN(clausolaIN, valoriStringa, creaParametriSql, injectionGuid)

    End Function
    Public Function Agro_SQL_SaveDate(ByVal DataItaliana As Date, Optional creaParametroSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String Implements ISqlSave.Agro_SQL_SaveDate

        If Not IsNothing(injectionGuid) Then
            creaParametroSql = True
        End If

        creaParametroSql = Me.ParametrizzaQuery
        Return _parametrizzatore.Agro_SQL_SaveDate(DataItaliana, creaParametroSql, injectionGuid)

    End Function

    Public Function Agro_SQL_SaveDateTime(ByVal DataOraItaliana As DateTime, Optional ByVal creaParametroSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String Implements ISqlSave.Agro_SQL_SaveDateTime
        If Not IsNothing(injectionGuid) Then
            creaParametroSql = True
        End If

        creaParametroSql = Me.ParametrizzaQuery
        Return _parametrizzatore.Agro_SQL_SaveDateTime(DataOraItaliana, creaParametroSql, injectionGuid)

    End Function

    Public Function Agro_SQL_SaveDateTime_NULL(ByVal item As Object, Optional ByVal creaParametroSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String Implements ISqlSave.Agro_SQL_SaveDateTime_NULL

        If Not IsNothing(injectionGuid) Then
            creaParametroSql = True
        End If

        creaParametroSql = Me.ParametrizzaQuery
        Return _parametrizzatore.Agro_SQL_SaveDateTime_NULL(item, creaParametroSql, injectionGuid)

    End Function

    Public Function Agro_SQL_SaveNum(ByVal StringaNumero As String, Optional creaParametroSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String Implements ISqlSave.Agro_SQL_SaveNum

        If Not IsNothing(injectionGuid) Then
            creaParametroSql = True
        End If

        creaParametroSql = Me.ParametrizzaQuery
        Return _parametrizzatore.Agro_SQL_SaveNum(StringaNumero, creaParametroSql, injectionGuid)

    End Function

    Public Function Agro_SQL_Save_xFiltroAggiuntivo(ByVal filtro As String,
                                                    Optional ByRef creaParametriSql As Boolean = True,
                                                    Optional ByVal injectionGuid As Guid = Nothing,
                                                    Optional ByVal objParametri As AgronicaCoreParametri = Nothing) As String Implements ISqlSave.Agro_SQL_Save_xFiltroAggiuntivo

        If Not IsNothing(injectionGuid) Then
            creaParametriSql = True
        End If

        creaParametriSql = Me.ParametrizzaQuery
        Return _parametrizzatore.Agro_SQL_Save_xFiltroAggiuntivo(filtro, creaParametriSql, injectionGuid, objParametri)

    End Function

    Public Function Agro_SQL_Save_xOrderBy(ByVal filtro As String, Optional ByVal objParametri As AgronicaCoreParametri = Nothing) As String Implements ISqlSave.Agro_SQL_Save_xOrderBy

        Return _parametrizzatore.Agro_SQL_Save_xOrderBy(filtro.ToOrigin(objParametri), objParametri)

    End Function


    Private Sub RimuoviParametri(ByRef parametri As Dictionary(Of Integer, AgroDBParametro), Optional ByVal identificatoreMetodo As String = "")

        If String.IsNullOrEmpty(identificatoreMetodo) OrElse identificatoreMetodo = Guid.Empty.ToString Then
            If parametri IsNot Nothing AndAlso parametri.Any Then
                parametri.Clear()
            End If
        Else
            Dim paramInjected = DataProviderFactory.Instance.ParametriInjected
            If paramInjected IsNot Nothing AndAlso paramInjected.ContainsKey(identificatoreMetodo) Then
                parametri.Clear()
                paramInjected.Remove(identificatoreMetodo)
            End If
        End If

    End Sub

    Public Function CreaParametriSQL() As Boolean

        If Me.TipoProvider = enum_DataProvidersType.OleDbProvider Then
            Return False
        End If

        Return Me.ParametrizzaQuery

    End Function

End Class
Public Class DataProviderFactoryAspx

    Public Function Factory() As DataProviderFactory
        Return DataProviderFactory.Instance
    End Function

End Class



Public Module DataProviderExtensions
    <Extension()>
    Public Function ToOrigin(ByVal sb As StringBuilder, Optional ByRef objParametri As AgronicaCoreParametri = Nothing) As String

        If sb Is Nothing OrElse sb.Length = 0 Then
            Return String.Empty
        End If

        Dim stringaSql = sb.ToString()
        Dim nomenclatore As New NomenclatoreParametri(objParametri)
        Return nomenclatore.RimuoviNomenclature(stringaSql)

    End Function

    <Extension()>
    Public Function ToOrigin(ByVal stringaSql As String, Optional ByRef objParametri As AgronicaCoreParametri = Nothing) As String

        If String.IsNullOrEmpty(stringaSql) Then
            Return String.Empty
        End If

        Dim nomenclatore As New NomenclatoreParametri(objParametri)
        Return nomenclatore.RimuoviNomenclature(stringaSql)

    End Function

    <Extension()>
    Public Function ToHash(ByVal stringaSql As String) As String

        Try
            Dim x As SHA512 = SHA512.Create()
            Dim hash = x.ComputeHash(Encoding.UTF8.GetBytes(stringaSql))
            Dim sb As New StringBuilder()
            For Each b As Byte In hash
                sb.AppendFormat("{0:X2}", b)
            Next

            Return sb.ToString

        Catch ex As Exception
            Return stringaSql
        End Try

    End Function

    <Extension()>
    Public Function ToOrigin(ByVal stringaSql As String, orderByEFiltro As OrderByFiltroAggiuntivo) As String

        If String.IsNullOrEmpty(stringaSql) Then
            Return String.Empty
        End If

        Dim nomenclatore As New NomenclatoreParametri

        Try
            If IsNothing(orderByEFiltro) Then
                Return nomenclatore.RimuoviNomenclature(stringaSql)
            Else

                Dim stringaOriginale As String = stringaSql

                If Not String.IsNullOrEmpty(orderByEFiltro.FiltroAggiuntivoOriginale) AndAlso Not String.IsNullOrEmpty(orderByEFiltro.FiltroAggiuntivoManipolato) Then
                    stringaOriginale = SostituisciSottostringa(stringaOriginale, orderByEFiltro.FiltroAggiuntivoManipolato, orderByEFiltro.FiltroAggiuntivoOriginale)
                End If

                If Not String.IsNullOrEmpty(orderByEFiltro.OrderByOriginale) AndAlso Not String.IsNullOrEmpty(orderByEFiltro.OrderByManipolato) Then
                    stringaOriginale = SostituisciSottostringa(stringaOriginale, orderByEFiltro.OrderByManipolato, orderByEFiltro.OrderByOriginale)
                End If
                Return nomenclatore.RimuoviNomenclature(stringaOriginale)

            End If
        Catch ex As Exception
            Return nomenclatore.RimuoviNomenclature(stringaSql)
        End Try

    End Function

    Private Function SostituisciSottostringa(ByVal stringaOriginale As String, ByVal stringaDaCercare As String, ByVal stringaDaSostituire As String) As String

        Dim retVal As String = stringaOriginale

        If stringaDaCercare.ToLowerInvariant().Equals(stringaDaSostituire.ToLowerInvariant) Then
            Return stringaOriginale
        End If

        Dim indiceNellaStringa As Integer = 0
        While indiceNellaStringa <> -1

            indiceNellaStringa = retVal.LastIndexOf(stringaDaCercare, StringComparison.InvariantCultureIgnoreCase)

            If indiceNellaStringa <> -1 Then
                retVal = retVal.Remove(indiceNellaStringa, stringaDaCercare.Length)
                retVal = retVal.Insert(indiceNellaStringa, stringaDaSostituire)
            End If

        End While

        Return retVal

    End Function
End Module

